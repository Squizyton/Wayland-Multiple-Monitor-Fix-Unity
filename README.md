# XWayland-Multiple-Monitor-Fix-Unity

## What is this.

Unity has a bug on Linux builds, specifically with XWayland and how XWayland treats multiple monitors.

The most left monitor is treated as 0,0, So when Unity opens windows, instead of putting it 0,0 with Unity, it put's the windows in the top left most corner of your monitor setup.

All this script does is alleviate that.

It forces Windows to snap to the middle of the Main window OR your mouse (which ever you prefer)


## Installation

Slap in Assets > Editor.

That's it.

## How to use?

Once in your Editor folder it should automatically be running. There are 4 settings that you can change. Each is explanatory. Just change the script from True/False on each value. There is no fancy editor script (yet)


# To-Do
- Make it more performant?
- Add Editor options inside the Project-Settings menu
- Remove window id from list if window closes

  # FAQ
  - Can I use this with Windows/MacOS?
  Yea, I guess. 

  - Does Unity Know about this
