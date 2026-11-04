public class Solution
{
    public int MinEatingSpeed(int[] piles, int h)
    {
        var low = 1;
        var high = piles.Max();

        while (low < high)
        {
            var mid = low + (high - low) / 2;
            long hours = 0;

            foreach (var pile in piles)
                hours += (pile + (long)mid - 1) / mid;

            if (hours <= h)
                high = mid;
            else
                low = mid + 1;
        }

        return low;
    }
}
