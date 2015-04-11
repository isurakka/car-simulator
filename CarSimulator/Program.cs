using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSimulator
{
    class SplineCurve
    {
        public Vector2f StartPoint;
        public Vector2f MiddlePoint;
        public Vector2f EndPoint;
        public float StartWidth;
        public float EndWidth;

        public SplineCurve(Vector2f startPoint, Vector2f middlePoint, Vector2f endPoint, float startWidth, float endWidth)
        {
            StartPoint = startPoint;
            MiddlePoint = middlePoint;
            EndPoint = endPoint;
            StartWidth = startWidth;
            EndWidth = endWidth;
        }

        public SplineCurve ToSim(SplineCurve spline)
        {
            return new SplineCurve(
                spline.StartPoint.ToSim(), 
                spline.MiddlePoint.ToSim(), 
                spline.EndPoint.ToSim(),
                spline.StartWidth.DisplayToSim(),
                spline.EndWidth.DisplayToSim());
        }

        public SplineCurve Translate(Vector2f translation)
        {
            StartPoint += translation;
            MiddlePoint += translation;
            EndPoint += translation;
            return this;
        }

        public Sample GetSample(float time)
        {
            var startToMiddle = (1f - time) * StartPoint + time * MiddlePoint;
            var middleToEnd = (1f - time) * MiddlePoint + time * EndPoint;
            return new Sample(
                (float)Math.Pow(1d - (double)time, 2d)*StartPoint + 2f*(1f - time)*time*MiddlePoint + (float)Math.Pow(time, 2d)*EndPoint,
                (middleToEnd - startToMiddle).Normalize().RotateDegrees(-90f).Normalize(),
                (1f - time)*StartWidth + time*EndWidth);
        }

        public struct Sample
        {
            public Vector2f Point;
            public Vector2f Normal;
            public float Width;
            
            public Sample(Vector2f point, Vector2f normal, float width)
            {
                Point = point;
                Normal = normal;
                Width = width;
            }
        }
    }

    class Program
    {
        const float step = 1f / 240f;
        static void Main(string[] args)
        {
            // Tuning parameters
            const float wheelDistance = 50f;
            const float wheelFrontOffset = 50f;
            const float sensorRadius = 1.5f;
            const float sensorSpacing = 3f;
            const float plateSensorOverflow = 10f;
            const float wheelWidth = 10f;
            const float wheelHeight = 25f;
            const float linearDamping = 3f;
            const float angularDamping = 8f;
            const float inertia = 1f;
            const float trackWidth = 5f;
            const float trackThickWidth = 12f;
            const int trackSampleDivision = 20;

            // Derived parameters
            var leftSensorLocalPos = new Vector2f(sensorRadius * -2f - sensorSpacing, 0f);
            var rightSensorLocalPos = new Vector2f(sensorRadius * 2f + sensorSpacing, 0f);
            var leftWheelLocalPos = new Vector2f(wheelDistance / -2f, wheelFrontOffset);
            var rightWheelLocalPos = new Vector2f(wheelDistance / 2f, wheelFrontOffset);

            // Init rendering
            var rw = new RenderWindow(new VideoMode(1024, 768), "Car Simulator", Styles.Close, new ContextSettings { AntialiasingLevel = 16 });
            //rw.SetVerticalSyncEnabled(true);
            rw.Closed += (s, a) => rw.Close();

            // Init physics
            var world = new World(new Vector2f());
            var carBody = new Body(world, new Vector2f(300f, 420f).DisplayToSim(), MathExtender.DegreeToRadian(80f));
            carBody.Inertia = inertia;
            carBody.IsBullet = true;
            carBody.LinearDamping = linearDamping;
            carBody.AngularDamping = angularDamping;
            var leftWheel = FixtureFactory.AttachRectangle(wheelWidth.DisplayToSim(), wheelHeight.DisplayToSim(), 1f,
                leftWheelLocalPos.DisplayToSim(), carBody);
            var rightWheel = FixtureFactory.AttachRectangle(wheelWidth.DisplayToSim(), wheelHeight.DisplayToSim(), 1f,
                rightWheelLocalPos.DisplayToSim(), carBody);
            var leftSensor = FixtureFactory.AttachCircle(sensorRadius.DisplayToSim(), 0f, carBody,
                leftSensorLocalPos.DisplayToSim());
            var middleSensor = FixtureFactory.AttachCircle(sensorRadius.DisplayToSim(), 0f, carBody, 
                new Vector2f().DisplayToSim());
            var rightSensor = FixtureFactory.AttachCircle(sensorRadius.DisplayToSim(), 0f, carBody,
                rightSensorLocalPos.DisplayToSim());
            var plate = FixtureFactory.AttachPolygon(
                new FarseerPhysics.Common.Vertices(new List<Vector2f> 
                { 
                    leftSensorLocalPos + new Vector2f(-plateSensorOverflow, -plateSensorOverflow),
                    rightSensorLocalPos + new Vector2f(plateSensorOverflow, -plateSensorOverflow),
                    rightWheelLocalPos + new Vector2f(wheelWidth / -2f, -wheelHeight / 2f),
                    rightWheelLocalPos + new Vector2f(wheelWidth / -2f, wheelHeight / 2f),
                    leftWheelLocalPos + new Vector2f(wheelWidth / 2f, wheelHeight / 2f),
                    leftWheelLocalPos + new Vector2f(wheelWidth / 2f, -wheelHeight / 2f),
                }.Select(v => v.ToSim().ToXNA())), 1f, carBody);
            carBody.BodyType = BodyType.Kinematic;
            var car = new Car(carBody);

            // Create track
            var trackSpline = new List<SplineCurve> 
            {
                new SplineCurve(new Vector2f(50f, 0f), new Vector2f(-50f, 0f), new Vector2f(-50f, 100f), trackWidth, trackWidth),
                new SplineCurve(new Vector2f(-50f, 100f), new Vector2f(-50f, 150f), new Vector2f(-50f, 200f), trackWidth, trackWidth),
                new SplineCurve(new Vector2f(-50f, 200f), new Vector2f(-50f, 300f), new Vector2f(50f, 300f), trackWidth, trackWidth),
                new SplineCurve(new Vector2f(50f, 300f), new Vector2f(75f, 300f), new Vector2f(100f, 300f), trackWidth, trackWidth),
                new SplineCurve(new Vector2f(100f, 300f), new Vector2f(115f, 300f), new Vector2f(130f, 300f), trackWidth, trackThickWidth),
                new SplineCurve(new Vector2f(130f, 300f), new Vector2f(330f, 300f), new Vector2f(430f, 300f), trackThickWidth, trackThickWidth),
                new SplineCurve(new Vector2f(430f, 300f), new Vector2f(445f, 300f), new Vector2f(460f, 300f), trackThickWidth, trackWidth),
                new SplineCurve(new Vector2f(460f, 300f), new Vector2f(560f, 300f), new Vector2f(560f, 200f), trackWidth, trackWidth),
                new SplineCurve(new Vector2f(560f, 200f), new Vector2f(560f, 150f), new Vector2f(560f, 100f), trackWidth, trackWidth),
                new SplineCurve(new Vector2f(560f, 100f), new Vector2f(560f, 0f), new Vector2f(460f, 0f), trackWidth, trackWidth),
                new SplineCurve(new Vector2f(460f, 0f), new Vector2f(380f, 0f), new Vector2f(320f, 0f), trackWidth, trackWidth),
                new SplineCurve(new Vector2f(320f, 0f), new Vector2f(292.5f, 0f), new Vector2f(265f, 30f), trackWidth, trackWidth),
                new SplineCurve(new Vector2f(265f, 30f), new Vector2f(237.5f, 60f), new Vector2f(210f, 60f), trackWidth, trackWidth),
                new SplineCurve(new Vector2f(210f, 60f), new Vector2f(182.5f, 60f), new Vector2f(155f, 30f), trackWidth, trackWidth),
                new SplineCurve(new Vector2f(155f, 30f), new Vector2f(127.5f, 00f), new Vector2f(100f, 0f), trackWidth, trackWidth),
                new SplineCurve(new Vector2f(100f, 0f), new Vector2f(75f, 0f), new Vector2f(50f, 0f), trackWidth, trackWidth),
            }.Select(sc => sc.Translate(new Vector2f(100f, 100f)));
            var trackBody = CreateTrack(world, trackSpline, trackSampleDivision);
            trackBody.IsBullet = true;
            trackBody.IsSensor = true;

            var time = 0f;
            var acc = 0f;
            //var step = 1f / 10000f;
            var sw = new Stopwatch();
            sw.Start();

            int lastTurn = 0;

            // Main loop
            while (rw.IsOpen)
            {
                rw.Clear(Color.White);

                rw.DispatchEvents();

                var dt = (float)new TimeSpan(sw.ElapsedTicks).TotalSeconds;
                sw.Restart();
                acc += dt;

                while (acc >= step)
                {
                    acc -= step;
                    time += step;

                    //Turn(-1, carBody, rightWheelLocalPos, leftWheelLocalPos);

                    if (!CheckSensor(leftSensor, trackBody))
                    {
                        lastTurn = -1;
                    }
                    else if (!CheckSensor(rightSensor, trackBody))
                    {
                        lastTurn = 1;
                    }

                    Turn(lastTurn, carBody, rightWheelLocalPos, leftWheelLocalPos);
                    

                    world.Step(step);
                }

                BodyRenderer.DrawBody(trackBody, rw, null, null, Color.Black);
                BodyRenderer.DrawFixture(leftWheel, rw, null, null, Color.Blue);
                BodyRenderer.DrawFixture(rightWheel, rw, null, null, Color.Blue);
                BodyRenderer.DrawFixture(plate, rw, null, null, new Color(140, 140, 140, 220));
                BodyRenderer.DrawFixture(leftSensor, rw, null, null, Color.Magenta);
                BodyRenderer.DrawFixture(middleSensor, rw, null, null, Color.Magenta);
                BodyRenderer.DrawFixture(rightSensor, rw, null, null, Color.Magenta);

                rw.Display();
            }
        }

        /*public static void Turn(int dir, Body body, Vector2f rightWheelLocalPos, Vector2f leftWheelLocalPos)
        {
            var sign = Math.Sign(dir);
            const float force = -1f;
            const float oppositeDiv = 1.018f;
            if (sign < 0)
            {
                body.ApplyForce(new Vector2f(0f, force).RotateRadians(body.Rotation), body.GetWorldPoint(rightWheelLocalPos));
                body.ApplyForce(new Vector2f(0f, force / oppositeDiv).RotateRadians(body.Rotation), body.GetWorldPoint(leftWheelLocalPos));
            }
            else if (sign > 0)
            {
                body.ApplyForce(new Vector2f(0f, force).RotateRadians(body.Rotation), body.GetWorldPoint(leftWheelLocalPos));
                body.ApplyForce(new Vector2f(0f, force / oppositeDiv).RotateRadians(body.Rotation), body.GetWorldPoint(rightWheelLocalPos));
            }
            else
            {
                body.ApplyForce(new Vector2f(0f, force).RotateRadians(body.Rotation), body.GetWorldPoint(rightWheelLocalPos));
                body.ApplyForce(new Vector2f(0f, force).RotateRadians(body.Rotation), body.GetWorldPoint(leftWheelLocalPos));
            }
        }*/

        public static void Turn(int dir, Body body, Vector2f rightWheelLocalPos, Vector2f leftWheelLocalPos)
        {
            var sign = Math.Sign(dir);
            const float force = -300f * step;
            const float rotate = 300f * step;
            if (sign < 0)
            {
                body.Position += new Vector2f(0f, force).ToSim().RotateRadians(body.Rotation);
                body.Rotation -= MathExtender.DegreeToRadian(rotate);
            }
            else if (sign > 0)
            {
                body.Position += new Vector2f(0f, force).ToSim().RotateRadians(body.Rotation);
                body.Rotation += MathExtender.DegreeToRadian(rotate);
            }
            else
            {
                body.Position += new Vector2f(0f, force).ToSim().RotateRadians(body.Rotation);
                //body.Rotation += MathExtender.DegreeToRadian(rotate);
            }
        }

        public static bool CheckSensor(Fixture sensor, Body trackBody)
        {
            var sensorWorldPoints = sensor.GetWorldVertices();
            foreach (var fix in trackBody.FixtureList)
            {
                var trackWorldPoints = fix.GetWorldVertices();
                foreach (var sensorPoint in sensorWorldPoints)
	            {
                    var refSensorPoint = sensorPoint;
                    var pip = trackWorldPoints.PointInPolygon(ref refSensorPoint);
                    if (pip == 1 || pip == 0)
                    {
                        return true;
                    }
	            }
            }

            return false;
        }

        public static Body CreateTrack(World world, IEnumerable<SplineCurve> spline, int sampleDivision)
        {
            var body = new Body(world);
            var timeAdd = 1f / sampleDivision;
            
            foreach (var curve in spline)
            {
                for (int i = 0; i < sampleDivision; i++)
                {
                    var curTime = 1f / sampleDivision * i;
                    var nextTime = 1f / sampleDivision * (i + 1);
                    var curSample = curve.GetSample(curTime);
                    var nextSample = curve.GetSample(nextTime);

                    FixtureFactory.AttachPolygon(new FarseerPhysics.Common.Vertices(new List<Vector2f> 
                    { 
                        curSample.Point + (-curSample.Normal) * curSample.Width,
                        curSample.Point + (curSample.Normal) * curSample.Width,
                        nextSample.Point + (nextSample.Normal) * nextSample.Width,
                        nextSample.Point + (-nextSample.Normal) * nextSample.Width,
                    }.Select(v => v.ToSim().ToXNA())), 1f, body);
                }
            }

            return body;
        }
    }
}
