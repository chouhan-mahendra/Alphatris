/**
    Sever for our word game,
**/

const PORT_WEBSOCKET = process.env.PORT || 3000;
const PORT_HTTP = 8080;

const express = require("express");
const app = express();

const socketIo = require("socket.io")(PORT_WEBSOCKET);
const utils = require("./utils.js");
const spellchecker = require("spellchecker");
const checkword = require("check-word")("en");
const { interval } = require('rxjs');
const { map, take } = require('rxjs/operators');

const GameActions = {
    init : "init",
    playerConnected : "playerConnected",
    playerDisconnected : "playerDisconnected",
    startGame : "startGame",
    wordSelected : "wordSelected",
    updateScore : "updateScore",
    destroyAlphabet : "destroyAlphabet", 
    initAlphabet : "initAlphabet"
};

app.get('/check', (req,res) => {
    const word = req.query.word.toLowerCase();
    console.log(req.query, !spellchecker.isMisspelled(word));
    res.send(spellchecker.isMisspelled(word) ? "0" : `${word.length}`);
});
app.listen(PORT_HTTP, () => console.log(`http server running on ${PORT_HTTP}`));

console.log(`websocket server starting on {${PORT_WEBSOCKET}}`);


const MIN_PLAYER_COUNT = 1;
const state = {};
socketIo.on("connection", socket => {

    const data = { 
                    id : utils.getRandomID(), 
                    name : utils.getRandomName(),
                    socket 
                };

    console.log(`client connected [${data.id} , ${data.name}]`);
    //for previous players
    socket.broadcast.emit(GameActions.playerConnected, { id : data.id, name : data.name });
    //for current player
    socket.emit(GameActions.init, { id : data.id, name : data.name });
    //simulate player connections for current player
    Object.keys(state).forEach(item => socket.emit(GameActions.playerConnected, { id : item.id, name : item.name }));
    state[data.id] = data;

    if(Object.keys(state).length == MIN_PLAYER_COUNT) {
        const event = { x : 2, char : 'Z' };
        console.log(`all players are ready, starting game with ${JSON.stringify(event)}`)
        Object.keys(state).forEach(player => {
            console.log(`starting game for ${player}, ${state[player].name}`);
            state[player].socket.emit(GameActions.startGame, event);
        });

        interval(3000).subscribe(counter => {
            const alphabet = {
                id : counter,
                x : Math.floor(Math.random() * 5), 
                char : utils.getRandomChar() 
            };
            //console.log(`sending ${JSON.stringify(alphabet)}`)
            Object.keys(state).forEach(player => {
                console.log(`sending ${JSON.stringify(alphabet)} to ${state[player].name}`);
                state[player].socket.emit(GameActions.initAlphabet, alphabet);
            });
        });
    }

    socket.on(GameActions.wordSelected, (event) => {
        console.log(`${data.name} selected ${event.word} [${spellchecker.isMisspelled(event.word)}]`);
        const { word } = event;
        if(!checkword.check(word)) 
           socket.emit(GameActions.updateScore, { score : 0 });         
        else 
           socket.emit(GameActions.updateScore, { score : word.length });       
    });

    socket.on("disconnect", () => {
        console.log(`client ${data.id} disconnected!!`);
        delete state[data.id];
        socket.broadcast.emit(GameActions.playerDisconnected, { id : data.id, name : data.name });
    });
});