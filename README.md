# GTA Online Keyboard Hook
Keyboard hook for GTA Online

The purpose of this application is to have a possibility easily win a car in GTA Online Casino's Happy Wheal

As may you know, the more you wait before spinning a wheel (before pressing 'S'), the wheel is being spined less.
Actually, to win a car, you need to wait approximately 4500ms before you press 'S' on your keyboard. The main problem is that
it's pretty hard to press 'S' on a keyboard right after N ms. This is what this application is made for - to spin the wheel automatically after 
N ms (the count is started after the notification 'To spin a wheel press S' is displayed). To get information about this notification,
the application checks the color of selected pixel every millisecond and when it changed to the needed one (RGB code of the color which is 
used for this notification is already set, but you can change it in app.config), program sends key down and up events to
GTA about pressing 'S' button.

Of course, I know, that the interface isn't really friendly, but mostly, this application was created just to get some experience and knowledge
with the WinApi32 library.
