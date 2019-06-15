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
const { map, skip, take } = require('rxjs/operators');
const redis = require('redis');

const GameActions = {
    init : "init",
    playerConnected : "playerConnected",
    playerDisconnected : "playerDisconnected",
    startGame : "startGame",
    wordSelected : "wordSelected",
    updateScore : "updateScore",
    destroyAlphabet : "destroyAlphabet", 
    spawnAlphabet : "spawnAlphabet",
    submitSelection: "submitSelection",
    addToPool: "addToPool",
    establishMultiplayerConnection: "establishMultiplayerConnection",
    multiplayerConnectionEstablished: "multiplayerConnectionEstablished"
};

app.get('/check', (req,res) => {
    const word = req.query.word.toLowerCase();
    console.log(req.query, !spellchecker.isMisspelled(word));
    const result = (spellchecker.isMisspelled(word) || word.length < 2) ? "0" : `${word.length}`;
    res.send(result);
});

app.listen(PORT_HTTP, () => console.log(`http server running on ${PORT_HTTP}`));
var players = {};
var multiAvailable = [];

console.log(`websocket server starting on {${PORT_WEBSOCKET}}`);

const MIN_PLAYER_COUNT = 2;
const state = {};
let subscription = undefined;

socketIo.on("connection", socket => {

    if (Object.keys(state).length == MIN_PLAYER_COUNT) {
        socket.close();
        return;
    }

    const data = { 
                    id : utils.getRandomID(), 
                    name : utils.getRandomName(),
                    socket 
                };
    players[data.id] = {"name": data.name};

    console.log(`client connected [${data.id} , ${data.name}]`);
    //for previous players
    socket.broadcast.emit(GameActions.playerConnected, { id : data.id, name : data.name });
    //for current player
    var multiId = "-1";
    if(multiAvailable.length != 0) {
        multiId = multiAvailable[0];
    }
    socket.emit(GameActions.init, { id : data.id, name : data.name, multiplayer_id: multiId });

    //simulate player connections for current player
    Object.keys(state).forEach(item => socket.emit(GameActions.playerConnected, { id : item.id, name : item.name }));
    state[data.id] = data;

    if(Object.keys(state).length == MIN_PLAYER_COUNT) {
        const event = { id : 0, x : 2, char : 'Z' };
        console.log(`all players are ready, starting game with ${JSON.stringify(event)}`)
        Object.keys(state).forEach(player => {
            console.log(`starting game for ${player}, ${state[player].name}`);
            state[player].socket.emit(GameActions.startGame, event);
        });

        subscription = interval(3000).subscribe(counter => {
            const alphabet = {
                id : counter + 1,
                x : Math.floor(Math.random() * 5), 
                char : utils.getRandomChar(),
                isSpecial: utils.isSpecial()
            };
            Object.keys(state).forEach(player => {
                console.log(`sending ${JSON.stringify(alphabet)} to ${state[player].name}`);
                state[player].socket.emit(GameActions.spawnAlphabet, alphabet);
            });
        });
    }

    socket.on(GameActions.addToPool, (event) => {
        const {id} = event;
        console.log(id);
        multiAvailable.push(id);
    });

    socket.on(GameActions.establishMultiplayerConnection, (event) => {
        const {multiplayerId} = event;
        var mulIndex = multiAvailable.map(function(e) { return e; }).indexOf(multiplayerId);
        multiAvailable = multiAvailable.splice(mulIndex, 1);
        players[multiplayerId].playing = data.id;
        players[data.id].playing = multiplayerId;
        socket.emit(GameActions.multiplayerConnectionEstablished, {id1: data.id, id2: multiplayerId});
    })

    socket.on(GameActions.submitSelection, (event) => {
        const { word , alphabetList, isDrag } = event;
        console.log(`${data.name} selected ${event.word}, ${alphabetList} [${spellchecker.isMisspelled(event.word)}], ${isDrag}`);

        const score = getScore(word, alphabetList, isDrag);
        let idList = [];
        for(let x in alphabetList) {
            idList.add(x.id);
        }
        if(score > 0) {
            socket.emit(GameActions.destroyAlphabet, { idList });
            socket.emit(GameActions.updateScore, {score});
        }
    });

    var getScore = (word, alphabetList, isDrag) => {
        if (spellchecker.isMisspelled(word) || word.length < 2) {
            return 0;
        }
        return word.length;
    }

    socket.on("disconnect", () => {
        console.log(`client ${data.id} disconnected!!`);
        delete state[data.id];
        socket.broadcast.emit(GameActions.playerDisconnected, { id : data.id, name : data.name });
        delete players[data.id];
        var ind = multiAvailable.map(function(e) { return e; }).indexOf(data.id);
        if(ind != -1) {
            multiAvailable = multiAvailable.splice(ind, 1);
        }
        if(subscription) {
            subscription.unsubscribe();
            subscription = undefined;
        }
    });
});