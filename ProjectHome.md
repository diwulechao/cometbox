## Purpose ##
To handle the concurrent connections needed for comet (server to client push) interaction in AJAX applications by queuing and sending messages to client browsers.


## Concept ##
![http://matt.injustice.net.nz/cometbox/diagram1.gif](http://matt.injustice.net.nz/cometbox/diagram1.gif)

  * **cometbox**
    * cometbox is an application written in C#.
      * Console, MS.NET and mono compatible.
      * Maybe someday C or C++ but not for now.
    * Runs on the server.
    * Queues messages for clients.
  * **Server Side**
    * Your choice of server side language.
    * I personally use PHP and will refer to to this part as PHP.
  * **User\Browser**
    * The users choice of standard XMLHttpRequest capable web browser.
      * Firefox, IE6, IE7, Safari. Maybe others, I'm not sure yet.
    * Uses javascript and a hefty dose of creativity.
  * **(1)**
    * An HTTP connection made from the browser via XMLHttpRequest.
    * cometbox accepts the connection and holds it until the browser breaks it off.
    * Upon connection, it sends any pending messages.
    * When cometbox receives a message for the connection from PHP, it sends it through the held connection.
    * Sends the messages in the form of a JSON array.
  * **(2)**
    * A short life span connection from PHP that sends data to cometbox's queue.
    * Uses XML formated messages.


## Parts ##
In order of development importance:
  * Application
    * Core application, in Mono C#.
  * PHP Class
    * Class using PHP sockets to connect to the server app.
    * Uses curl if installed, or fopen otherwise.
  * [Javascript Class](jsclass.md)
    * Class to abstract interaction with the server.
    * Should handle browser quirks that pop up so that theres no hassle to use it.
  * PHP Module
    * More ideal than a PHP class using sockets but requires more knowledge that I have at this point.

## Why? ##
My main reason is PHP sucks for comet but that is its only real lack for use as an AJAX platform. (Note: This application would be potentially usable with ANY server side application.)
To host comet you must have a connection open per active client. In PHP that would mean one loading page per user at all times, at 2-3mb per process this adds up very rapidly.

## What is Comet? ##
In short comet is a method using only javascript and the http protocol to allow a server to send data to the client without it being requested.

Traditionally data could only be given to a browser when requested by that browser. This led to frequent polling of the server which causes high requests as well as slow actual response time.

Comet utilizes the XMLHttpRequest to connect to a server side resource that holds the connection open and sends down data as it is received by the server in chunks. This allows for low latency server to client push without resource eating polling.