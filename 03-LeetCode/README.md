# Binary Search Problems

- Name: Ahmed Ismail
- Public LeetCode profile: https://leetcode.com/u/ahmed1kaballo/

## 875. Koko Eating Bananas

- Accepted submission: https://leetcode.com/problems/koko-eating-bananas/submissions/2189309695

I binary-search speeds from `1` to the largest pile. For a candidate speed I add
the rounded-up hours needed for every pile. If the total fits in `h`, the speed is
fast enough, so I keep it and everything smaller by moving `high` to `mid`.
Otherwise I move `low` above `mid`. When both meet, no slower feasible speed can
remain. Time is `O(n log m)` where `m` is the largest pile; extra space is `O(1)`.

Example 1, `piles = [3,6,7,11]`, `h = 8`:

| low | mid | high | hours | decision                      |
| --: | --: | ---: | ----: | ----------------------------- |
|   1 |   6 |   11 |     6 | fast enough, keep 6 and lower |
|   1 |   3 |    6 |    10 | too slow, discard through 3   |
|   4 |   5 |    6 |     8 | fast enough, keep 5 and lower |
|   4 |   4 |    5 |     8 | fast enough; answer is 4      |

## 33. Search in Rotated Sorted Array

- Accepted submission: https://leetcode.com/problems/search-in-rotated-sorted-array/submissions/2189310839

I binary-search index range `0` through `n - 1`. At least one half around `mid`
is sorted. I first identify that half, then check whether the target lies inside
its inclusive/exclusive bounds. I keep that half if it can contain the target;
otherwise I discard it. Every iteration removes at least half the remaining
indexes, so the target is returned or the range becomes empty. Time is `O(log n)`
and extra space is `O(1)`.

Example 1, `nums = [4,5,6,7,0,1,2]`, `target = 0`:

| low | mid | high | value | decision                                        |
| --: | --: | ---: | ----: | ----------------------------------------------- |
|   0 |   3 |    6 |     7 | left is sorted; target is not there, move right |
|   4 |   5 |    6 |     1 | left is sorted; target is there, move left      |
|   4 |   4 |    4 |     0 | found at index 4                                |
