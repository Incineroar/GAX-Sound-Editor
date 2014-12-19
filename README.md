GAX-Sound-Editor
================

Sound Editor for GAX-Powered Sound Engine Games. It's probably really broken right now, so take this merely as a proof-of concept that hardly even works.

Right now, there are two different loading methods implemented:

1. One where the offset of a sound table is known, referred to as LoadFormat 0, and

2. One where there is no table, so direct pointers to data footer blocks are used, referred to as LoadFormat 1.

There is a preliminary INI file available here: http://pastebin.com/ASgP5C3j. Put this in with the application under the name "games.ini".

Games currently supported in the INI are:

-Crash Bandicoot 2: N-Tranced (U)

-Crash Bandicoot - The Huge Adventure (U) (Partial, working on this!)

-Sigma Star Saga (U) (Incomplete, I'm still working on grabbing all the footer pointers!)

Take a look at these games and the code to see how each loading method works.

TODO:
==============
1. Add comments

2. Fix and add loading methods and reading methods

3. Support more GAX games
