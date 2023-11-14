using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HeavenSnake
{
    /// <summary>
    /// Snake Part
    /// </summary>
    internal class Part
    {
        /// <summary>
        /// The Part always follows the parent
        /// </summary>
        public Part Parent { get; set; }
        /// <summary>
        /// The x and y Position of the Part
        /// </summary>
        public Program.Vector2INT Position { get; set; }
        /// <summary>
        /// moves the Part to the parents position
        /// </summary>
        public void moveToParent()
        {
            Position = Parent.Position;
        }

        public Part(Part Parent)
        {
            this.Parent = Parent;
        }
    }
}
