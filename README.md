# beamOS
An open source, test-first structural analysis program

[![Coverage Status](https://coveralls.io/repos/github/connorivy/beamOS/badge.svg?branch=main)](https://coveralls.io/github/connorivy/beamOS?branch=main)

### DISCLAIMER : THIS PROJECT IS A WORK-IN-PROGRESS AND SHOULD NOT BE USED FOR DESIGNING ANY REAL-WORLD STRUCTURES

DISCLAIMER #2 : Up to this point, most of the effort has been put toward the structural analysis logic and API design. You'll find that the user interface is seriously in need of more attention.

To run this project, simply:
1. clone or fork the repo
1. install and use npm version >= 20
1. open the .sln in your IDE of choice and build the project
1. set BeamOS.WebApp, BeamOS.PhysicalModel.Api, and BeamOS.DirectStiffnessMethod.Api as startup projects
1. run the project

#### What to expect

You will be greeted with a lacking (although I prefer the term minimal) UI that shows a simple default structure which is automatically seeded to the SQL Server database.

![Screenshot 2024-01-02 093126](https://github.com/connorivy/beamOS/assets/43247197/57ed5ce8-227d-4dc2-b327-47822def42a3)


This structure has the exact geometry, restraints, loads, and sections as example problem 3.8 in Matrix Analysis Of Structures 2nd Ed.
 by Prof. Aslam Kassimali, though many of those elements are not reflected in the UI.

![Screenshot 2024-01-02 094019](https://github.com/connorivy/beamOS/assets/43247197/5a660b84-38b8-4781-b535-b61ff00c7cd2)

 You will notice there are two tables underneath the editor that show calculated reactions and displacements compared to expected values. The calculated values will be blank and the expected values come from the aforementioned textbook.

 Clicking the "Solve" button will call the DirectStiffnessMethod api, populate the calculated values, and populate the "Difference" column to show that the calculated values equal the expected values.

 ![ezgif-4-fd9163ea5c](https://github.com/connorivy/beamOS/assets/43247197/0c188e37-c8ee-4252-a7d6-4ab146923051)

  The editor has some functionality that allows you to move the nodes, but currently this functionality is not yet wired up to the DB so these changes won't actually change anything in the analysis.

 ![ezgif-4-0285652186](https://github.com/connorivy/beamOS/assets/43247197/3848de76-a6e0-4354-8453-b55898fade70)

