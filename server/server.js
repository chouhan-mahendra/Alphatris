const PORT = process.env.PORT || 3000;
const socketIo = require("socket.io")(PORT);
const utils = require("./utils.js");

const GameActions = {
    init : "init",
    playerConnected : "playerConnected",
    playerDisconnected : "playerDisconnected",
    startGame : "startGame"
};

console.log(`server started {${PORT}}`);

const state = {};

socketIo.on("connection", socket => {

    const data = { 
                    id : utils.getRandomID(), 
                    name : utils.getRandomName(),
                    channel : socket 
                };

    console.log(`client connected [${data.id} , ${data.name}]`);
    //for previous players
    socket.broadcast.emit(GameActions.playerConnected, { id : data.id, name : data.name });
    //for current player
    socket.emit(GameActions.init, { id : data.id, name : data.name });
    //simulate player connections for current player
    Object.keys(state).forEach(item => socket.emit(GameActions.playerConnected, { id : item.id, name : item.name }));
    state[data.id] = data;

    if(Object.keys(state).length > 1) 
        socketIo.local.emit(GameActions.startGame,{});
    // socket.on(GameActions.move, (data) => {
    //     console.log("move :" + JSON.stringify(data));
    //     state[data.id] = data;
    //     socket.broadcast.emit(GameActions.move, data);
    //     socket.emit(GameActions.move, data);
    // });

    socket.on("disconnect", () => {
        console.log(`client ${data.id} disconnected!!`);
        delete state[data.id];
        socket.broadcast.emit(GameActions.playerDisconnected, { id : data.id, name : data.name });
    });
});