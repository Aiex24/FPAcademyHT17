using System;
using System.Collections.Generic;
using System.Text;

namespace WindCatchersMathLibrary
{
    public static class InterpolationLogic
    {
        //Det här ska göras med varje specifik TWS.
        /// <summary>
        /// Interpolates VPP from the values that have been input. 
        /// The vpp array should have a length of 181 to account for each degree needed.
        /// </summary>
        /// <param name="vpp"></param>
        public static void VppInterpolation(decimal[] vpp)
        {
            //Vpp diagram with values for one TWN for each angle from 0 to 180 [degrees]

            //the lowest angle the yacht can sail towards the wind
            int minDeg = 30;

            //// Här ska värdena in från ViewModel OBS: Behövs inte då arrayen är en referenstyp, så jag smackar in en go array vid metodanropet.
            //Vpp[31] = 0;
            //Vpp[60] = 5;
            //Vpp[90] = 10;
            //Vpp[120] = 15;
            //Vpp[150] = 2;
            ////-------------------

            // y0 = knot1 , y1 = knot2, x0 = degree1, x1 = degree2   =>  interpolate to find y for each x
            decimal y0 = 0;
            decimal y1 = 0;
            int x0 = 0;
            int x1 = 0;

            //First angle that have a defined value, based on what data the User has given 
            int firstDeg = 0;
            decimal firstKn = 0;

            //Second angle that have a value, based on what the User has given
            int secondDeg = 0;
            decimal secondKn = 0;

            //Temp variable for the loop to only find the first value once
            bool b = false;
            //Checks if a value is defined for the first angle towards the wind that the yacht can sail towards
            bool first = false;

            int lastx0 = 0;
            decimal lasty0 = 0;

            //Checks if value for the first angle towards the wind a yacht can sail towards, is defined
            if (vpp[minDeg + 1] == 0)
            {
                first = true;
            }

            //Creates values for each degree from 0 to 180 and store those in the VPP diagram (array)
            for (int i = minDeg + 1; i < 180 + 1; i++)
            {
                if (vpp[i] != 0)
                {
                    //Will execute each time a y value is found, that is over zero except the first time
                    if (b)
                    {
                        y1 = vpp[i];
                        x1 = i;
                        decimal[] yArr = new decimal[x1 - x0 - 1];

                        yArr = Interpolate(x0, x1, y0, y1);
                        for (int j = 0; j < yArr.Length; j++)
                        {
                            vpp[j + 1 + x0] = yArr[j];
                        }
                        lastx0 = x0;
                        lasty0 = y0;
                        y0 = y1;
                        x0 = x1;
                        if (first)
                        {
                            secondDeg = x1;
                            secondKn = y1;
                            first = false;
                        }
                    }

                    //Will only execute for the first time a y value is found that is over zero
                    else
                    {
                        y0 = vpp[i];
                        x0 = i;
                        b = true;

                        if (first)
                        {
                            firstDeg = x0;
                            firstKn = y0;
                        }
                    }
                }
            }

            //If the graph is not defined for the end point, each y1 value will be calculated
            if (vpp[180] == 0)
            {
                int x = x1;
                decimal y = y1;
                x0 = lastx0;
                y0 = lasty0;

                //Interpolation Finds all y1 values for x1 based on the linear equation between x0 and x if an end value was not defined
                for (int x11 = lastx0 + 1; x11 < 180 + 1; x11++)
                {
                    decimal value = vpp[x11] = (x * y0 - x0 * y + x11 * y - x11 * y0) / (x - x0);

                    if (value <= 0)
                    {
                        value = 0;
                    }
                    vpp[x11] = value;
                }
            }

            //Interpolation Finds all y0 values for x0 based on the linear equation between x and x1 if a start value was not defined
            if (vpp[minDeg + 1] == 0)
            {
                decimal y = firstKn;
                y1 = secondKn;
                int x = firstDeg;
                x1 = secondDeg;

                for (int x01 = minDeg + 1; x01 < x; x01++)
                {

                    decimal value = (x01 * y + x * y1 - x01 * y1 - x1 * y) / (x - x1);
                    if (value <= 0)
                    {
                        value = 0;
                    }
                    vpp[x01] = value;
                }

            }
        }

        private static decimal[] Interpolate(int x0, int x1, decimal y0, decimal y1)
        {
            decimal[] resArr = new decimal[x1 - x0 - 1];

            for (int x = x0 + 1; x < resArr.Length + x0 + 1; x++)
            {
                resArr[x - x0 - 1] = (x * y0 - x * y1 + x0 * y1 - x1 * y0) / (x0 - x1);
            }

            return resArr;
        }
    }
}
