using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Diagnostics;

namespace Snake
{
    class Snake
    {
        private Queue<Point> body;
        private Point food;
        private int width, height;
        private Random rand;
        private bool duplicates;
        private Direction prev;

        public Snake(int width, int height)
        {
            rand = new Random();
            body = new Queue<Point>();

            this.width = width;
            this.height = height;

            duplicates = false;

            randomize();
            newFood();
        }

        private void randomize()
        {
            int x = rand.Next(4, width - 4);
            int y = rand.Next(4, height - 4);
            Point head = new Point(x, y);
            body.Enqueue(head);
        }

        private void newFood()
        {
            int x = rand.Next(0, width);
            int y = rand.Next(0, height);
            food = new Point(x, y);

            if (body.Contains(food))
                newFood();
        }

        public Point getFood()
        {
            return food;
        }

        public Point[] getBody()
        {
            return body.ToArray();
        }

        public void move(Direction dir)
        {
            Point next = getMove(body.Last(), dir);
            
            if (body.Contains(next))
                duplicates = true;

            body.Enqueue(next);

            if (isEating())
                newFood();
            else
                body.Dequeue();

            prev = dir;
        }

        private bool isEating()
        {
            return body.Last().Equals(food);
        }

        public bool isDead()
        {
            Point head = body.Last();

            if (duplicates)
                return true;

            return head.X >= width || head.X < 0 || head.Y >= height || head.Y < 0;
        }

        public int getLength()
        {
            return body.Count;
        }

        private Point getMove(Point orig, Direction dir)
        {
            Point ret = new Point(orig.X, orig.Y);

            if (dir == Direction.DOWN)
                ret.Y += 1;
            else if (dir == Direction.UP)
                ret.Y -= 1;
            else if (dir == Direction.LEFT)
                ret.X -= 1;
            else if (dir == Direction.RIGHT)
                ret.X += 1;

            return ret;
        }
    }
    public enum Direction
    {
        UP = 0,
        DOWN = 1,
        LEFT = 2,
        RIGHT = 3
    }
}
