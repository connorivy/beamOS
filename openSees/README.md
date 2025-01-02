In order to get the OpenSees analysis working, you must download OpenSees. You can go to [their website](https://opensees.berkeley.edu/wiki/index.php/Getting_Started_with_OpenSees_--_Download_OpenSees) to view the download instructions.

Unzip the downloaded zip file and you will find two folders named "bin" and "lib". Copy these two folders and all of their contents into this directory (beamOS/openSees/).

That's it. There is a post-build step in the BeamOS.WebApp.csproj that will copy these files to the correct place.
