var app = require('express')();
var http = require('http').Server(app);
var io = require('socket.io')(http);
var array = require('array');

var users = array();

var rooms = array();



app.get('/', function (req, res) {
    res.sendfile('chat/index.html');
});

io.on('connection', function (client) {

    client.on('join', function (user) {

        client.name = user.name;
        client.avatar = user.avatar;
        client.room = user.room;
        client.join(user.room);
        console.log(user.name + ' connected to room: ' + client.room);

        if (rooms.indexOf(client.room) == -1) {
            rooms.push(client.room);
            users.push(array([client.name]));
        }
        else {
            var index = rooms.indexOf(client.room);
            users[index].push(client.name);
        }

        client.broadcast.to(client.room).emit('user_joined_chat', client.name);
        client.emit('active-users', users[rooms.indexOf(client.room)]);
    });

    client.on('disconnect', function () {
        if (client.name != null) {
            console.log(client.name + ' disconnected');
            var index = rooms.indexOf(client.room);
            var uindex = users[index].indexOf(client.name);
            users[index][uindex] = null;
            users[index] = users[index].filter(function (item) {
                return (item != null);
            });

            io.in(client.room).emit('user_left_chat', client.name);
        }
    });

    client.on('video_link', function (video) {
        var videolink = { name: client.name, video: video, avatar: client.avatar };
        io.in(client.room).emit('AppendVideoLink', videolink);
        console.log('[' + client.room + '] ' + client.name + ' sent a youtube link: https://www.youtube.com/watch?v=' + video);
    });

    client.on('chat_message', function (msg) {
        var message = { name: client.name, message: msg, avatar: client.avatar };
        io.in(client.room).emit('AppendChatMessage', message);
        console.log('[' + client.room + '] ' + client.name + ' says: ' + msg);
    });

    client.on('ApproveConversation', function () {
        io.in(client.room).emit('ApproveConversation');
    });

});

http.listen(3000, function () {
    console.log('listening on *:3000');
});