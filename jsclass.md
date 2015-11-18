# Introduction #

Intended to handle all interaction with the comet host so that the end developer doesn't have to worry about any problems.

# Basic Idea #

#### The Class ####
```
var cometbox = {
  poll_interval: 30, //interval to poll for information when holding a connection wont work.

  // initialize the class
  // url - url to the cometbox host
  // id - users cometbox id
  // callback - function called when data is received 
  init: function(url, id, callback) {
   
  },

  //stop waiting for information
  stop: function() {
    
  },

  //not gonna bother thinking up internals at this point
  _internalCrap: function() {

  }
}
```

#### Usage ####
```
function cometboxCallback(data) {
  //do stuff with data
}

cometbox.init('http://www.blah.com:8181/', 'theusersid', cometboxCallback);
```


