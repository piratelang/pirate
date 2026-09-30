# Runtime Errors (RTN)

Errors in this category occur while the program is executing. These are
caught by the VM and reported with the same source-excerpt format as
compile-time errors.

*(Not yet implemented — documented for completeness.)*

Planned additions when the VM lands: **RTN-004** — the module `pirate run`
resolves as the fleet entry point has no top-level statements to execute
(reported today as a plain CLI message, not yet coded).

---

### RTN-001 — Division by zero

```
main.pirate:3:11 Division by zero *RTN-001*
  2 | func main() : void {
  3 |     var x = 1 / 0;
    |           ^
```

**Cause**: An integer or float division uses zero as the divisor.
**Fix**: Ensure the divisor is non-zero, or add a guard.

### RTN-002 — Index out of bounds

```
main.pirate:3:5 Index out of bounds (index 5, length 3) *RTN-002*
  2 |     int[] arr = [1, 2, 3];
  3 |     arr[5] = 0;
    |     ^
```

**Cause**: An array index is negative or exceeds the array's length.
**Fix**: Check the index value before access.

### RTN-003 — Null reference

```
main.pirate:3:5 Null reference *RTN-003*
  2 |     var arr = null;
  3 |     arr[0] = 1;
    |     ^
```

**Cause**: An operation is attempted on a null reference.
**Fix**: Ensure the value is non-null before use.
