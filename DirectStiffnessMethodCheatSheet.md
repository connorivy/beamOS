### Variable Definitions
d -> Joint Displacements

F -> Member global end force vector

k -> Member Stiffness Matrix In Local Coordinate System

K -> Member Stiffness Matrix In Global Coordinate System

P -> External Joint Loads

Q -> Member Local End Force Vector

Qf -> Member Local Fixed-End Force Vector

R -> Joint Reactions

S -> Structure Stiffness Matrix

 A structure stiffness coefficient Sij represents the force at the location and in the direction of Pi required, along with other joint forces, to cause a unit value of the displacement dj, while all other joint displacements are zero. S is a square matrix with the number of rows and columns equal to the degrees of freedom (NDOF)

u -> Member local end displacement vector

v -> Member global end displacement vector

### Equations

Q = ku + Qf
 The total forces Q that can develop at the ends of a member can be expressed as the sum of the forces ku due to the end displacements u, and the fixed-end forces Qf that would develop at the member ends due to external loads if both member ends were fixed against translations and rotations.

 Q = TF

 P = Sd

 F = Ku


Sign Convention
The sign convention for member local fixed-end forces, Qf, is the same as that for the member end forces in the local coordinate system, Q. Thus, the member local fixed-end axial forces and shears are considered positive when in the positive directions of the member’s local x and y axes, and the local fixed-end moments are considered positive when counterclockwise. However, the member loads are commonly defined to be positive in the directions opposite to those for the local fixed-end forces. In other words, the member axial and perpendicular loads are considered positive when in the negative directions of the member’s local x and y axes, respectively, and the external couples applied to the members are considered positive when clockwise. The expressions for the member fixed end forces (including moments) given inside the front cover of this text are based on this sign convention, in which all the fixed-end forces and member loads (including couples) are shown in the positive sense.
