# Bugs

- Name: Ahmed Ismail

| Rule | Failing test | Expected | Actual before fix | Fix |
| --- | --- | --- | --- | --- |
| BR-03 | `CalculateRentalDays_GivenValidSpan_AppliesGraceHour(1500, 1)` | Exactly 25 hours counts as one day | It counted two days | Changed the remainder check from `>= 60` to `> 60` |

The fix is intentionally not assigned a fake commit hash. The real hash should be
added after the test and fix are committed in the required order.
