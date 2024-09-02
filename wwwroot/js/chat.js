"use strict";

fetch("/api/get-user-is-login", {method: "GET"})
.then(response => response.json())
.then(data => {
    if(data.login === true){
        var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub")
                            .configureLogging(signalR.LogLevel.Information)
                            .build();
        var timer;
        
        // start connection
        connection.start().then(function (){ 
            fetch("/api/get-current-user-login", {method: "GET"})
            .then(response => response.json())
            .then(data => {
                if(data.success == true){
                    // add id user online in _connectionService
                    connection.invoke("UserConnectionChat", data.id, data.name);
                }
                else{ console.error("Get User False!") }
            })
            .catch(error => console.error(error));
        })
        .catch(function (err){ 
            return console.error(err.toString())
        });

        // handle when client receive message
        connection.on("ReceiveMessage", function (username, id, message, hash){
            // only show message when user are in page /chat
            if(window.location.pathname === "/chat"){
                var idUserChat = document.getElementById("toid-input").value; // get current id user is chatting
                var curuserid = document.getElementById("fromid-input").value;

                // if not user is chatting with current user, dont show line message.
                if(idUserChat == id){
                    connection.invoke("UserHaveReadMessage", curuserid, idUserChat);
                    CreateElementLiMessage(username, hash["DateCreate"], message, "");
                    ScrollTopListMessage();
                    // active icon if user have message from other user
                    document.getElementById(`have-mess-${username}`).hidden = true;
                }
                else{
                    document.getElementById(`have-mess-${username}`).hidden = false;
                }
            }
            CheckUserHaveMessageNotRead();
        });

        CheckUserHaveMessageNotRead();
        // update icon chat on top menu when user have message
        function CheckUserHaveMessageNotRead()
        {
            var urlAction = "/api/check-user-have-message";
            $.ajax({
                cache: false,
                async: true,
                url: urlAction,
                type: "POST",
                contentType: false,
                processData: false,
                success: function(data)
                {
                    if(data.havemess === true){
                        document.getElementById("layout-icon-chat").hidden = false;
                    }
                    else{
                        document.getElementById("layout-icon-chat").hidden = true;
                    }
                },
                error: function (xhr) {
                    alert('error');
                }
            });
        }

        // // when client close connection
        // connection.onclose(error => {
        //     console.assert(connection.state === signalR.HubConnectionState.Disconnected);
        // });

        // // when client reconnecting hub
        // connection.onreconnecting(error => {
        //     console.assert(connection.state === signalR.HubConnectionState.Reconnecting);
        // });

        // // when client have reconnected 
        // connection.onreconnected(connectionId  => {
        //     console.assert(connection.state === signalR.HubConnectionState.Connected);
        // });

        if(window.location.pathname === "/chat"){
            // handle event click button send to send message
            document.getElementById("sendButton").addEventListener("click", function(event){
                var message = document.getElementById("messageInput").value;
                if(message.length === 0) { return; }

                var username = document.getElementById("current-user").value;
                var fromid = document.getElementById("fromid-input").value;
                var toid = document.getElementById("toid-input").value;
                var stringids = `${fromid}&&${toid}`;

                connection.invoke("SendMessage", stringids, message).then(function (){
                    var datetime =  new Date().toLocaleString();
                    document.getElementById("messageInput").value = ''; // clear message input
                    CreateElementLiMessage(username, datetime, message, "blue");
                    ScrollTopListMessage();
                })
                .catch(function (err){
                    return console.error(err.toString());
                });
                event.preventDefault();
            });
            ReloadUserOnline();
        }

        function ReloadUserOnline()
        {
            timer = setTimeout(() => {
                UpdateUserConnectionChat();
                ReloadUserOnline();
            }, 10000);
        }

        // window.navigation.addEventListener("navigate", (event) => {
        //     clearTimeout(timer);
        //     connection.stop();
        // });

        // stop connection when user close browser or tab or reload
        window.addEventListener('beforeunload', function (e) {
            // e.preventDefault();
            clearTimeout(timer);
            connection.stop();
        });

        // stop connection when user log out
        try {
            document.getElementById("btn-logoff-layout").addEventListener("click", function (){
                clearTimeout(timer);
                connection.stop();
            });
        }
        catch(err) {
            document.getElementById("btn-logoff-layoutadmin").addEventListener("click", function (){
                clearTimeout(timer);
                connection.stop();
            });
        }
    }
})
.catch(error => console.error(error));






