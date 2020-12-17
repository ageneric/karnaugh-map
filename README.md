# Karnaugh Map: Digital Logic
C# and Unity: a graphical Karnaugh map solver, for up to 5-bit inputs.

## Truth table
Input a logical expression to view its truth table.
If 2 - 5 variables are used, the truth table will be used to populate the Karnaugh map.

## Karnaugh map
The input length can be changed by selecting a grid size from the right.
For each input bit, the first number shown is the index; the second number is on/off.

The Karnaugh map is solved with a custom algorithm (probably less time-efficient
than QM) and the loops (essential prime implicants) are drawn to the screen.
