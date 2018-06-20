using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace IOHandling
{
    /// <summary>
    /// 비프음으로 사운드를 핸들링한다.
    /// </summary>
    public class SoundHandling
    {
        /// <summary> 
        /// 비프음을 내는 시스템 함수
        /// </summary>
        /// <param name="freq">주파수</param>
        /// <param name="dur">비프음 길이(시간, 단위 : 1000 = 1초)</param>
        [DllImport("KERNEL32.DLL")]
        extern public static void Beep(int freq, int dur);
        // 도 = 256Hz
        // 레 = 도 * 9/8 = 288Hz
        // 미 = 레 * 10/9 = 320Hz
        // 파 = 미 * 16/15 = 341.3Hz
        // 솔 = 파 * 9/8 = 384Hz
        // 라 = 솔 * 10/9 = 426.6Hz
        // 시 = 라 * 9/8 = 480Hz
        // 도 = 시 * 16/15 = 512Hz (= 처음 도의 2배)
        // 2배 = 높은음, 1/2배 = 낮은음

        /// <summary>
        /// 음을 내는 함수.
        /// </summary>
        /// <param name="freq">주파수</param>
        /// <param name="level">얼마나 증폭할지. 양수는 같은 음계의 높은음, 음수는 낮은 음이다.</param>
        /// <param name="timeInMs">지속시간. ms단위</param>
        public void Sound(double freq, double level = 0, int timeInMs = 300)
        {
            //if (level == 0)
            Beep((int)(freq * Math.Pow(2, level)), timeInMs);
            /*
            else if (level > 0)
                Beep(freq * level, timeInMs);
            else
                Beep(freq / level, timeInMs);
             */
        }

        /// <summary>
        /// 도
        /// </summary>
        /// <param name="level">기본음은 0, 높은 음은 양수로, 낮은 음은 음수로</param>
        /// <param name="timeInMs">지속시간</param>
        public void Do(double level = 0, int timeInMs = 300)
        {
            Sound(256, level, timeInMs);
        }
        /// <summary>
        /// 레
        /// </summary>
        /// <param name="level">기본음은 0, 높은 음은 양수로, 낮은 음은 음수로</param>
        /// <param name="timeInMs">지속시간</param>
        public void Re(double level = 0, int timeInMs = 300)
        {
            Sound(288, level, timeInMs);
        }

        /// <summary>
        /// 미
        /// </summary>
        /// <param name="level">기본음은 0, 높은 음은 양수로, 낮은 음은 음수로</param>
        /// <param name="timeInMs">지속시간</param>
        public void Mi(double level = 0, int timeInMs = 300)
        {
            Sound(325, level, timeInMs);
        }

        /// <summary>
        /// 파
        /// </summary>
        /// <param name="level">기본음은 0, 높은 음은 양수로, 낮은 음은 음수로</param>
        /// <param name="timeInMs">지속시간</param>
        public void Pa(double level = 0, int timeInMs = 300)
        {
            Sound(345, level, timeInMs);
        }

        /// <summary>
        /// 솔
        /// </summary>
        /// <param name="level">기본음은 0, 높은 음은 양수로, 낮은 음은 음수로</param>
        /// <param name="timeInMs">지속시간</param>
        public void Sol(double level = 0, int timeInMs = 300)
        {
            Sound(384, level, timeInMs);
        }

        /// <summary>
        /// 라
        /// </summary>
        /// <param name="level">기본음은 0, 높은 음은 양수로, 낮은 음은 음수로</param>
        /// <param name="timeInMs">지속시간</param>
        public void Ra(double level = 0, int timeInMs = 300)
        {
            Sound(426.6, level, timeInMs);
        }

        /// <summary>
        /// 시
        /// </summary>
        /// <param name="level">기본음은 0, 높은 음은 양수로, 낮은 음은 음수로</param>
        /// <param name="timeInMs">지속시간</param>
        public void Si(double level = 0, int timeInMs = 300)
        {
            Sound(480, level, timeInMs);
        }
    }
}
