# beamOS
An open source, test-first structural analysis program

[![Coverage Status](https://coveralls.io/repos/github/connorivy/beamOS/badge.svg?branch=main)](https://coveralls.io/github/connorivy/beamOS?branch=main)

### DISCLAIMER : THIS PROJECT IS A WORK-IN-PROGRESS AND SHOULD NOT BE USED FOR DESIGNING ANY REAL-WORLD STRUCTURES

DISCLAIMER #2 : Up to this point, most of the effort has been put toward the structural analysis logic and API design. You'll find that the user interface is seriously in need of more attention.

To run this project, simply:
1. clone or fork the repo
1. install and use npm version >= 20
1. build the project IN RELEASE MODE
    - building the project in release mode will trigger an npm install and populate a 'js' folder under the wwwroot of the WebApp project
1. *optional*: build and run 'BeamOs.CodeGen.TestResults' project in Release mode.
    - warning: this will take roughly 15 minutes
    - this will generate the testing reports (code coverage and mutation score) that are linked on the test explorer dashboard
1. set BeamOS.WebAppas startup project and run

#### Testing

To run the webapp integration tests:
```bash
export BEAMOS_ROOT=$(pwd)
./scripts/test-webapp.sh
```

For detailed testing documentation, see [docs/WEBAPP_TESTING.md](docs/WEBAPP_TESTING.md).

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

 #### License

 beamOS is published under the Apache 2.0 license:

```
Copyright (C) 2025 Connor Ivy <connor@beamos.net>

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   https://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
```

 beamOS uses a [CSparse.NET](https://github.com/wo80/CSparse.NET) which is a csharp port of the [CSparse](https://github.com/DrTimothyAldenDavis/SuiteSparse/tree/dev/CSparse/Source) library, published under the LGPL-2.1+ license:

```
CSparse: a Concise Sparse matrix package.
Copyright (c) 2006, Timothy A. Davis.
http://www.suitesparse.com

--------------------------------------------------------------------------------

CSparse is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

CSparse is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this Module; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
```
