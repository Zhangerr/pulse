Pulse: a rhythm game using OpenTK (OpenGL wrapper), C#, BASS.NET, and Lua
====
Pulse was actively developed from 2011-2012 and in its final stages had online capabilities. A summary of some of its features are as follows:

-  integrated with IRC (people in game could chat with people connecting to the irc channel via traditional clients and vice versa), 
-  a custom web site that 
    -  allowed users to submit custom content called charts, which contained a song usually in mp3 form and corresponding user created input data that would generate the notes falling down the screen
    -  showed chart submissions
    -  showed scoreboards for each of these user submitted charts
-  a forum that once registered the same details could be used to login in game and on the custom website
-  a custom server that allowed spectating and watching replays and viewing scoreboards in game
-  a high level of customizability: users could replace visual elements of the user interface as they see fit and distribute these customizations as 'skins. Used Lua to define customizations to allow programmatic randomization or generation
-  an custom updater that compared md5 hashes to do incremental updates, only updating the changed, and a tool to push out updates easily
-  in game editor that allowed users to create charts while playing, pausing, rewinding, and fast forwarding music (lines represented snapping)
-  custom file format for charts with parser and generator

However, due to unfortunate circumstances, the server and the corresponding services it hosted (web site/forums, irc, the pulse server) were shut down and deleted
 
I've been trying to get Pulse running on my new computer, but it seems the Lua interpreter we're using has some dependency that my old computer somehow satisfied but my new one cannot, even after installing the required C++ redistributable.

### Technologies (make links)
-  C#
-  [OpenTK](http://www.opentk.com/) (OpenGL Wrapper, used for all graphics)
-  [BASS.NET](http://www.un4seen.com/) (Audio library)
-  [Lua](http://www.lua.org/) (via [LuaInterface](https://code.google.com/p/luainterface/), used for skinning) 
-  [Ionic Zip](http://dotnetzip.codeplex.com/), used for reading charts

### Resources
-  Stackoverflow (so many questions answered)
-  OpenTK forums (more esoteric questions)


### Gameplay Videos

Chronologically ordered

Earliest version with lackluster graphics, but basic concept of gameplay existed

http://www.youtube.com/watch?v=T8Iw6ROR3Cg

Next iteration, graphics slightly improved and more visible

http://www.youtube.com/watch?v=vwAnm_6ZTzI

Further down the line, with revamped menu screen

http://www.youtube.com/watch?v=o3AydGJgHEk

One of the final builds, nice looking in game animations

http://www.youtube.com/watch?v=hwhB4Sii6ZI








