/**
    Sever for our word game,
**/

const PORT_WEBSOCKET = process.env.PORT || 3000;
const PORT_HTTP = 8080;

const express = require("express");
const app = express();

const socketIo = require("socket.io")(PORT_WEBSOCKET);
const utils = require("./utils.js");
const checkword = require("check-word")("en");
const { interval } = require('rxjs');
const { map, skip, take } = require('rxjs/operators');

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
    multiplayerConnectionEstablished: "multiplayerConnectionEstablished",
    invalidSelection: "invalidSelection",
    playerReady: "playerReady",
    checkAndDestroyAlphabet: "checkAndDestroyAlphabet",
    reset: "reset"
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
let subscription = undefined;

socketIo.on("connection", socket => {

    const data = { 
                    id : utils.getRandomID(), 
                    name : utils.getRandomName(),
                    socket 
                };
    players[data.id] = data;

    console.log(`client connected [${data.id} , ${data.name}]`);
    

    var multiId = "-1";
    if(multiAvailable.length != 0) {
        multiId = multiAvailable[0];
    }
    socket.emit(GameActions.init, { id : data.id, name : data.name, multiplayer_id: multiId });

    socket.on(GameActions.addToPool, (event) => {
        const {id} = event;
        console.log(id);
        multiAvailable.push(id);
    });

    socket.on(GameActions.establishMultiplayerConnection, (event) => {
        const {multiplayerId} = event;
        var mulIndex = multiAvailable.map(function(e) { return e; }).indexOf(multiplayerId);
        multiAvailable = multiAvailable.splice(mulIndex, 1);
        if(!players.hasOwnProperty(multiplayerId)) {
            return ;
        }
        players[multiplayerId].playing = data.id;
        players[data.id].playing = multiplayerId;
        socket.emit(GameActions.multiplayerConnectionEstablished, {id1: data.id, id2: multiplayerId});
        players[multiplayerId].socket.emit(GameActions.playerReady);

        subscription = interval(3000).subscribe(counter => {
            const alphabet = {
                id : counter + 1,
                x : Math.floor(Math.random() * 5), 
                char : utils.getRandomChar(),
                isSpecial: utils.isSpecial()
            };
            players[data.id].socket.emit(GameActions.spawnAlphabet, alphabet);
            players[multiplayerId].socket.emit(GameActions.spawnAlphabet, alphabet);
        });
    })

    socket.on(GameActions.submitSelection, (event) => {
        const { word , wordList, isDrag, specials } = event;
        console.log(`${data.name} selected ${event.word}, ${idList} [${checkword.check(event.word.toLowerCase())}], ${isDrag}`);

        var idList = JSON.parse(wordList);
        var specialCount = parseInt(specials);
        const score = getScore(word, specialCount, isDrag);
        if(score > 0) {
            socket.emit(GameActions.updateScore, {score});
            var currPlayer = players[data.id];
            console.log(currPlayer.id);
            if(currPlayer.hasOwnProperty("playing")) {
                console.log(players[currPlayer["playing"]].id);
                players[currPlayer["playing"]].socket.emit(GameActions.checkAndDestroyAlphabet, {idList});
            }
        } else {
            socket.emit(GameActions.invalidSelection);
        }
    });

    var getScore = (word, specialCount, isDrag) => {
        if (checkword.check(word.toLowerCase()) /*&& word.length > 2*/) {
            return word.length;
        }
        return 0;
    }

    socket.on(GameActions.reset, (event) => {
        console.log("reseting client");
        delete players[data.id].playing
        if(subscription) {
            subscription.unsubscribe();
            subscription = undefined;
        }
    });

    socket.on("disconnect", () => {
        console.log(`client ${data.id} disconnected!!`);
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