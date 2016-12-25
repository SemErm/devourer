using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace devourer
{
    class Score
    {
        private int count;

        public Score()
        {
            count = 0;
        }

        public void Increment()
        {
            count++;
        }

        public int GetScore()
        {
            return count;
        }
    }
}
