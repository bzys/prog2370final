/*
 *  Author:     Bill Sun        
 *  Comments:   the house basement map class
 *  Revision History: 
 *      2018-12-9: latest
 *      2018-10-29: created
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BSunFinalProject
{
    class MapHouse : DrawableGameComponent
    {
        private Matrix cameraViewMatrix;
        private Matrix worldMatrix;
        private Vector3 modelPosition;

        //the rotations, degress translated into radians
        private float rotation0 = MathHelper.ToRadians(0);
        private float rotation90 = MathHelper.ToRadians(90);
        private float rotation180 = MathHelper.ToRadians(180);
        private float rotation270 = MathHelper.ToRadians(270);

        private Model mapHouseBasement;

        //the bounding boxes for the walls, there are two wall types, a 1x1 and 1x2
        private BoundingBox squareBoundingBox = new BoundingBox(new Vector3(-0.25f, 0.0f, -0.25f), new Vector3(0.25f, 2.5f, 0.25f));
        private BoundingBox rectangleBoundingBox = new BoundingBox(new Vector3(-0.5f, 0.0f, -0.25f), new Vector3(0.5f, 2.5f, 0.25f));

        //wall health
        private const int FULLWALLHP = 100;
        //for drawing bounding box visualizations
        BasicEffect boxEffect;

        private List<Walls> basementWalls;

        #region wallModels and wallList

        private Model wallConcrete1x;
        private Model wallConcrete1xCorner;
        private Model wallConcrete1xHalfCorner;
        private Model wallConcrete1xOutside;

        private Model wallConcrete2x;
        private Model wallConcrete2xOutside;

        private Model wallWood1xFullDmg;
        private Model wallWood1xHalfDmg;
        private Model wallWood1xNoDmg;
        private Model wallWood1xQuarterDmg;

        private Model wallWood2xFullDmg;
        private Model wallWood2xHalfDmg;
        private Model wallWood2xNoDmg;
        private Model wallWood2xQuarterDmg;

        #endregion

        //GraphicsDeviceManager graphics
        public MapHouse(Game game) : base(game)
        {
            //this.graphics = graphics;
            basementWalls = new List<Walls>();
            modelPosition = new Vector3(0, 0, 0);

            //for drawing bounding box visualizations
            boxEffect = new BasicEffect(game.GraphicsDevice);

            mapHouseBasement = game.Content.Load<Model>("mapHouseBase");

            //loading all the wall models
            wallConcrete1x = game.Content.Load<Model>("Walls/walls_Concrete1x");
            wallConcrete1xCorner = game.Content.Load<Model>("Walls/walls_Concrete1xCorner");
            wallConcrete1xHalfCorner = game.Content.Load<Model>("Walls/walls_Concrete1xHalfCorner");
            wallConcrete1xOutside = game.Content.Load<Model>("Walls/walls_Concrete1xOutside");

            wallConcrete2x = game.Content.Load<Model>("Walls/walls_Concrete2x");
            wallConcrete2xOutside = game.Content.Load<Model>("Walls/walls_Concrete2xOutside");

            wallWood1xFullDmg = game.Content.Load<Model>("Walls/walls_Wood1xFullDmg");
            wallWood1xHalfDmg = game.Content.Load<Model>("Walls/walls_Wood1xHalfDmg");
            wallWood1xNoDmg = game.Content.Load<Model>("Walls/walls_Wood1xNoDmg");
            wallWood1xQuarterDmg = game.Content.Load<Model>("Walls/walls_Wood1xQuarterDmg");

            wallWood2xFullDmg = game.Content.Load<Model>("Walls/walls_Wood2xFullDmg");
            wallWood2xHalfDmg = game.Content.Load<Model>("Walls/walls_Wood2xHalfDmg");
            wallWood2xNoDmg = game.Content.Load<Model>("Walls/walls_Wood2xNoDmg");
            wallWood2xQuarterDmg = game.Content.Load<Model>("Walls/walls_Wood2xQuarterDmg");

            #region adding walls to list of walls

            //this is all the walls of the basement map, 188 in total. each wall class takes a position, bounding box, type of wall, isbreakable bool and rotation float
            basementWalls.Add(new Walls(new Vector3(0.75f, 0.0f, 0.25f), squareBoundingBox, WALLTYPE.CONCRETE1XHALFCORNER, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(0.25f, 0.0f, 0.25f), squareBoundingBox, WALLTYPE.CONCRETE1XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(-0.25f, 0.0f, 0.25f), squareBoundingBox, WALLTYPE.CONCRETE1XOUTSIDE, false, rotation270));

            basementWalls.Add(new Walls(new Vector3(-0.25f, 0.0f, -0.50f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(-0.25f, 0.0f, -1.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(-0.25f, 0.0f, -2.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(-0.25f, 0.0f, -3.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(-0.25f, 0.0f, -4.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(-0.25f, 0.0f, -5.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(-0.25f, 0.0f, -6.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(-0.25f, 0.0f, -7.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(-0.25f, 0.0f, -8.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(-0.25f, 0.0f, -9.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));

            basementWalls.Add(new Walls(new Vector3(-0.25f, 0.0f, -10.25f), squareBoundingBox, WALLTYPE.CONCRETE1XOUTSIDE, false, rotation180));
            basementWalls.Add(new Walls(new Vector3(0.5f, 0.0f, -10.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation180));
            basementWalls.Add(new Walls(new Vector3(1.5f, 0.0f, -10.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation180));
            basementWalls.Add(new Walls(new Vector3(2.5f, 0.0f, -10.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation180));
            basementWalls.Add(new Walls(new Vector3(3.5f, 0.0f, -10.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation180));

            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -10.25f), squareBoundingBox, WALLTYPE.CONCRETE1X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -9.5f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation90));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -8.5f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation90));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -7.75f), squareBoundingBox, WALLTYPE.CONCRETE1XCORNER, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -4.75f), squareBoundingBox, WALLTYPE.CONCRETE1XCORNER, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -4.0f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation90));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -3.0f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation90));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -2.0f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation90));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -1.0f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation90));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -0.25f), squareBoundingBox, WALLTYPE.CONCRETE1X, false, rotation90));

            //upper-left most walls along the edge
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -11.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -12.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -13.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -14.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -15.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -16.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -17.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -18.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -19.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -20.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -21.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -22.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -23.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -24.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -25.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));

            //internal wall between garage and workout room
            basementWalls.Add(new Walls(new Vector3(4.75f, 0.0f, -13.75f), squareBoundingBox, WALLTYPE.CONCRETE1X, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(5.5f, 0.0f, -13.75f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(6.5f, 0.0f, -13.75f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(7.5f, 0.0f, -13.75f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(8.5f, 0.0f, -13.75f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(9.5f, 0.0f, -13.75f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(10.5f, 0.0f, -13.75f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(11.25f, 0.0f, -13.75f), squareBoundingBox, WALLTYPE.WOOD1XNODMG, true, rotation180));
            basementWalls.Add(new Walls(new Vector3(11.75f, 0.0f, -13.75f), squareBoundingBox, WALLTYPE.CONCRETE1X, false, rotation0));

            //internal walls of centre stairs in workout room
            basementWalls.Add(new Walls(new Vector3(12.25f, 0.0f, -9.25f), squareBoundingBox, WALLTYPE.CONCRETE1XCORNER, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(12.25f, 0.0f, -10.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(12.25f, 0.0f, -11.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(12.25f, 0.0f, -12.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(12.25f, 0.0f, -13.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(12.25f, 0.0f, -14.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(12.25f, 0.0f, -15.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(12.25f, 0.0f, -15.75f), squareBoundingBox, WALLTYPE.CONCRETE1X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(12.25f, 0.0f, -16.25f), squareBoundingBox, WALLTYPE.CONCRETE1XHALFCORNER, false, rotation90));

            //wall along the top
            basementWalls.Add(new Walls(new Vector3(4.0f, 0.0f, 0.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(3.25f, 0.0f, 0.25f), squareBoundingBox, WALLTYPE.CONCRETE1XHALFCORNER, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(5.0f, 0.0f, 0.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(6.0f, 0.0f, 0.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(7.0f, 0.0f, 0.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(8.0f, 0.0f, 0.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(8.75f, 0.0f, 0.25f), squareBoundingBox, WALLTYPE.CONCRETE1XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(9.25f, 0.0f, 0.25f), squareBoundingBox, WALLTYPE.CONCRETE1XHALFCORNER, false, rotation0));

            basementWalls.Add(new Walls(new Vector3(9.25f, 0.0f, 1.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(9.25f, 0.0f, 2.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));

            //walls along the top of map
            basementWalls.Add(new Walls(new Vector3(10.0f, 0.0f, 2.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(11.0f, 0.0f, 2.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(12.0f, 0.0f, 2.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(13.0f, 0.0f, 2.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(14.0f, 0.0f, 2.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(14.75f, 0.0f, 2.25f), squareBoundingBox, WALLTYPE.CONCRETE1XOUTSIDE, false, rotation0));

            //walls between workout room and side stairs
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, 3.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, 2.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, 1.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, 0.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, -0.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, -1.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, -2.25f), squareBoundingBox, WALLTYPE.CONCRETE1X, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, -2.75f), squareBoundingBox, WALLTYPE.CONCRETE1XCORNER, false, rotation90));

            //walls between centre stairs and laundry room
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, -5.75f), squareBoundingBox, WALLTYPE.CONCRETE1XHALFCORNER, false, rotation180));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, -6.25f), squareBoundingBox, WALLTYPE.CONCRETE1X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, -7.0f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation90));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, -8.0f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation90));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, -9.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, -10.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, -11.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, -12.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, -13.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, -14.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, -15.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(15.25f, 0.0f, -15.75f), squareBoundingBox, WALLTYPE.CONCRETE1X, false, rotation90));

            //walls between laundry room and garage
            basementWalls.Add(new Walls(new Vector3(13.0f, 0.0f, -16.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(14.0f, 0.0f, -16.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(15.0f, 0.0f, -16.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(15.75f, 0.0f, -16.25f), squareBoundingBox, WALLTYPE.CONCRETE1X, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(16.5f, 0.0f, -16.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(17.5f, 0.0f, -16.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(18.5f, 0.0f, -16.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(19.25f, 0.0f, -16.25f), squareBoundingBox, WALLTYPE.CONCRETE1XCORNER, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(22.25f, 0.0f, -16.25f), squareBoundingBox, WALLTYPE.CONCRETE1XCORNER, false, rotation180));
            basementWalls.Add(new Walls(new Vector3(23.0f, 0.0f, -16.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation180));
            basementWalls.Add(new Walls(new Vector3(24.0f, 0.0f, -16.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation180));
            basementWalls.Add(new Walls(new Vector3(25.0f, 0.0f, -16.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation180));

            //x-positive laundry room walls
            basementWalls.Add(new Walls(new Vector3(25.25f, 0.0f, -15.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(25.25f, 0.0f, -14.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(25.25f, 0.0f, -13.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(25.25f, 0.0f, -12.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(25.25f, 0.0f, -11.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(25.25f, 0.0f, -10.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(25.25f, 0.0f, -9.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(25.25f, 0.0f, -8.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(25.25f, 0.0f, -7.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(25.25f, 0.0f, -6.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(25.25f, 0.0f, -5.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(25.25f, 0.0f, -4.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(25.25f, 0.0f, -3.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));

            //walls north of laundry room
            basementWalls.Add(new Walls(new Vector3(25.0f, 0.0f, -2.75f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(24.0f, 0.0f, -2.75f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(23.0f, 0.0f, -2.75f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(22.0f, 0.0f, -2.75f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(21.25f, 0.0f, -2.75f), squareBoundingBox, WALLTYPE.CONCRETE1XHALFCORNER, false, rotation90));

            //side stair walls
            basementWalls.Add(new Walls(new Vector3(21.25f, 0.0f, -2.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(21.25f, 0.0f, -1.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(21.25f, 0.0f, 0.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(21.25f, 0.0f, 1.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(21.25f, 0.0f, 2.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(21.25f, 0.0f, 3.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(21.25f, 0.0f, 4.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));

            //side stair top walls
            basementWalls.Add(new Walls(new Vector3(20.5f, 0.0f, 4.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(19.5f, 0.0f, 4.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(18.5f, 0.0f, 4.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(17.5f, 0.0f, 4.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(16.5f, 0.0f, 4.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(15.5f, 0.0f, 4.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation0));

            //walls between side stairs and laundry room
            basementWalls.Add(new Walls(new Vector3(16.0f, 0.0f, -5.75f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(17.0f, 0.0f, -5.75f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(18.0f, 0.0f, -5.75f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(19.0f, 0.0f, -5.75f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(20.0f, 0.0f, -5.75f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(20.75f, 0.0f, -5.75f), squareBoundingBox, WALLTYPE.CONCRETE1X, false, rotation0));
            basementWalls.Add(new Walls(new Vector3(21.25f, 0.0f, -5.75f), squareBoundingBox, WALLTYPE.CONCRETE1XCORNER, false, rotation0));

            //garage walls - positive x side
            basementWalls.Add(new Walls(new Vector3(22.75f, 0.0f, -17f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(22.75f, 0.0f, -18f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(22.75f, 0.0f, -19f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(22.75f, 0.0f, -20f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(22.75f, 0.0f, -21f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(22.75f, 0.0f, -22f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(22.75f, 0.0f, -23f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(22.75f, 0.0f, -24f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(22.75f, 0.0f, -25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(22.75f, 0.0f, -26f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(22.75f, 0.0f, -27f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(22.75f, 0.0f, -28f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(22.75f, 0.0f, -29f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(22.75f, 0.0f, -30f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation90));

            //garage walls - minus z side
            basementWalls.Add(new Walls(new Vector3(22.25f, 0.0f, -30.25f), squareBoundingBox, WALLTYPE.CONCRETE1XOUTSIDE, false, rotation180));
            basementWalls.Add(new Walls(new Vector3(21.5f, 0.0f, -30.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(20.5f, 0.0f, -30.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(19.5f, 0.0f, -30.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(18.5f, 0.0f, -30.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(17.5f, 0.0f, -30.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(16.5f, 0.0f, -30.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(15.5f, 0.0f, -30.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(14.5f, 0.0f, -30.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(13.5f, 0.0f, -30.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(12.5f, 0.0f, -30.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation180));
            basementWalls.Add(new Walls(new Vector3(11.75f, 0.0f, -26.25f), squareBoundingBox, WALLTYPE.CONCRETE1XOUTSIDE, false, rotation180));
            basementWalls.Add(new Walls(new Vector3(11.0f, 0.0f, -26.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(10.0f, 0.0f, -26.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(9.0f, 0.0f, -26.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(8.0f, 0.0f, -26.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(7.0f, 0.0f, -26.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(6.0f, 0.0f, -26.25f), rectangleBoundingBox, WALLTYPE.WOOD2XNODMG, true, rotation0));
            basementWalls.Add(new Walls(new Vector3(5.0f, 0.0f, -26.25f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation180));
            basementWalls.Add(new Walls(new Vector3(4.25f, 0.0f, -26.0f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));

            //garage walls - divider
            basementWalls.Add(new Walls(new Vector3(12.25f, 0.0f, -29.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(12.25f, 0.0f, -28.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(12.25f, 0.0f, -27.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(12.25f, 0.0f, -26.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2XOUTSIDE, false, rotation270));
            basementWalls.Add(new Walls(new Vector3(12.25f, 0.0f, -25.5f), rectangleBoundingBox, WALLTYPE.CONCRETE2X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(12.25f, 0.0f, -24.75f), squareBoundingBox, WALLTYPE.CONCRETE1X, false, rotation90));
            basementWalls.Add(new Walls(new Vector3(12.25f, 0.0f, -24.25f), squareBoundingBox, WALLTYPE.CONCRETE1XCORNER, false, rotation270));

            #endregion
        }

        public override void Draw(GameTime gameTime)
        {
            //drawing basement floor texture
            foreach (ModelMesh mesh in mapHouseBasement.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                    effect.World = Matrix.Identity;
                    
                    effect.World = Matrix.CreateTranslation(modelPosition);
                    effect.World = Matrix.CreateRotationY(rotation90); //90 degree rotation from default

                    //worldMatrix = effect.World;

                    effect.View = cameraViewMatrix;
                    effect.Projection = _Initializations.GetProjectionMatrix();
                }
                mesh.Draw();
            }
            //looping through each wall and drawing them, this is pretty dirty copy-paste determine which wall to draw if code
            foreach (Walls wall in basementWalls)
            {
                //this code is for drawing bounding box visualization
                //drawBoundingBox(wall);

                if (wall.WallType == WALLTYPE.CONCRETE1X)
                {
                    foreach (ModelMesh mesh in wallConcrete1x.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;

                            effect.World = _Initializations.GetWorldMatrix();
                            effect.World *= Matrix.CreateRotationY(wall.WallRotation); //getting the rotation of the wall 
                            effect.World *= Matrix.CreateTranslation(wall.WallPosition);

                            worldMatrix = effect.World;

                            effect.View = cameraViewMatrix;
                            effect.Projection = _Initializations.GetProjectionMatrix();
                        }
                        mesh.Draw();
                    }
                }
                if (wall.WallType == WALLTYPE.CONCRETE1XCORNER)
                {
                    foreach (ModelMesh mesh in wallConcrete1xCorner.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;

                            effect.World = _Initializations.GetWorldMatrix();
                            effect.World *= Matrix.CreateRotationY(wall.WallRotation); //getting the rotation of the wall 
                            effect.World *= Matrix.CreateTranslation(wall.WallPosition);
                            
                            worldMatrix = effect.World;

                            effect.View = cameraViewMatrix;
                            effect.Projection = _Initializations.GetProjectionMatrix();
                        }
                        mesh.Draw();
                    }
                }
                if (wall.WallType == WALLTYPE.CONCRETE1XHALFCORNER)
                {
                    foreach (ModelMesh mesh in wallConcrete1xHalfCorner.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;

                            effect.World = _Initializations.GetWorldMatrix();
                            effect.World *= Matrix.CreateRotationY(wall.WallRotation); //getting the rotation of the wall 
                            effect.World *= Matrix.CreateTranslation(wall.WallPosition);

                            worldMatrix = effect.World;

                            effect.View = cameraViewMatrix;
                            effect.Projection = _Initializations.GetProjectionMatrix();
                        }
                        mesh.Draw();
                    }
                }
                if (wall.WallType == WALLTYPE.CONCRETE1XOUTSIDE)
                {
                    foreach (ModelMesh mesh in wallConcrete1xOutside.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;

                            effect.World = _Initializations.GetWorldMatrix();
                            effect.World *= Matrix.CreateRotationY(wall.WallRotation); //getting the rotation of the wall 
                            effect.World *= Matrix.CreateTranslation(wall.WallPosition);

                            worldMatrix = effect.World;

                            effect.View = cameraViewMatrix;
                            effect.Projection = _Initializations.GetProjectionMatrix();
                        }
                        mesh.Draw();
                    }
                }
                if (wall.WallType == WALLTYPE.CONCRETE2X)
                {
                    foreach (ModelMesh mesh in wallConcrete2x.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;

                            effect.World = _Initializations.GetWorldMatrix();
                            effect.World *= Matrix.CreateRotationY(wall.WallRotation); //getting the rotation of the wall 
                            effect.World *= Matrix.CreateTranslation(wall.WallPosition);
                            
                            worldMatrix = effect.World;

                            effect.View = cameraViewMatrix;
                            effect.Projection = _Initializations.GetProjectionMatrix();
                        }
                        mesh.Draw();
                    }
                }
                if (wall.WallType == WALLTYPE.CONCRETE2XOUTSIDE)
                {
                    foreach (ModelMesh mesh in wallConcrete2xOutside.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;

                            effect.World = _Initializations.GetWorldMatrix();
                            effect.World *= Matrix.CreateRotationY(wall.WallRotation); //getting the rotation of the wall 
                            effect.World *= Matrix.CreateTranslation(wall.WallPosition);

                            worldMatrix = effect.World;

                            effect.View = cameraViewMatrix;
                            effect.Projection = _Initializations.GetProjectionMatrix();
                        }
                        mesh.Draw();
                    }
                }

                //this is for deciding which of the breakable walls to draw based on their remaining health, there are 3 different "damaged" states based on wall hp
                if (wall.IsDestroyable)
                {
                    //if wall is partially damaged, hp is between 0.75 and 0.4
                    if (wall.WallHealth <= (FULLWALLHP * 0.75) && wall.WallHealth >= (FULLWALLHP * 0.4))
                    {
                        if (wall.WallType == WALLTYPE.WOOD1XNODMG)
                            wall.WallType = WALLTYPE.WOOD1XQUARTERDMG;
                        if (wall.WallType == WALLTYPE.WOOD2XNODMG)
                            wall.WallType = WALLTYPE.WOOD2XQUARTERDMG;
                    }
                    //if wall is more damaged, hp is between 0.4 and above 0
                    else if (wall.WallHealth <= (FULLWALLHP * 0.4) && wall.WallHealth >= 1)
                    {
                        if (wall.WallType == WALLTYPE.WOOD1XQUARTERDMG || wall.WallType == WALLTYPE.WOOD1XNODMG)
                            wall.WallType = WALLTYPE.WOOD1XHALFDMG;
                        if (wall.WallType == WALLTYPE.WOOD2XQUARTERDMG || wall.WallType == WALLTYPE.WOOD2XNODMG)
                            wall.WallType = WALLTYPE.WOOD2XHALFDMG;
                    }
                    //if wall hp falls to or is below 0, wall is dead
                    else if (wall.WallHealth < 1)
                    {
                        if (wall.WallType == WALLTYPE.WOOD1XHALFDMG || wall.WallType == WALLTYPE.WOOD1XQUARTERDMG || wall.WallType == WALLTYPE.WOOD1XNODMG)
                            wall.WallType = WALLTYPE.WOOD1XFULLDMG;
                        if (wall.WallType == WALLTYPE.WOOD2XHALFDMG || wall.WallType == WALLTYPE.WOOD2XQUARTERDMG || wall.WallType == WALLTYPE.WOOD2XNODMG)
                            wall.WallType = WALLTYPE.WOOD2XFULLDMG;

                        wall.CanWalkThrough = true; //if wall is fully destroyed, player can walk through
                    }
                }

                if (wall.WallType == WALLTYPE.WOOD1XFULLDMG)
                {
                    foreach (ModelMesh mesh in wallWood1xFullDmg.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;

                            effect.World = _Initializations.GetWorldMatrix();
                            effect.World *= Matrix.CreateRotationY(wall.WallRotation); //getting the rotation of the wall 
                            effect.World *= Matrix.CreateTranslation(wall.WallPosition);

                            worldMatrix = effect.World;

                            effect.View = cameraViewMatrix;
                            effect.Projection = _Initializations.GetProjectionMatrix();
                        }
                        mesh.Draw();
                    }
                }
                if (wall.WallType == WALLTYPE.WOOD1XHALFDMG)
                {
                    foreach (ModelMesh mesh in wallWood1xHalfDmg.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;

                            effect.World = _Initializations.GetWorldMatrix();
                            effect.World *= Matrix.CreateRotationY(wall.WallRotation); //getting the rotation of the wall 
                            effect.World *= Matrix.CreateTranslation(wall.WallPosition);

                            worldMatrix = effect.World;

                            effect.View = cameraViewMatrix;
                            effect.Projection = _Initializations.GetProjectionMatrix();
                        }
                        mesh.Draw();
                    }
                }
                if (wall.WallType == WALLTYPE.WOOD1XNODMG)
                {
                    foreach (ModelMesh mesh in wallWood1xNoDmg.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;

                            effect.World = _Initializations.GetWorldMatrix();
                            effect.World *= Matrix.CreateRotationY(wall.WallRotation); //getting the rotation of the wall 
                            effect.World *= Matrix.CreateTranslation(wall.WallPosition);

                            worldMatrix = effect.World;

                            effect.View = cameraViewMatrix;
                            effect.Projection = _Initializations.GetProjectionMatrix();
                        }
                        mesh.Draw();
                    }
                }
                if (wall.WallType == WALLTYPE.WOOD1XQUARTERDMG)
                {
                    foreach (ModelMesh mesh in wallWood1xQuarterDmg.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;

                            effect.World = _Initializations.GetWorldMatrix();
                            effect.World *= Matrix.CreateRotationY(wall.WallRotation); //getting the rotation of the wall 
                            effect.World *= Matrix.CreateTranslation(wall.WallPosition);

                            worldMatrix = effect.World;

                            effect.View = cameraViewMatrix;
                            effect.Projection = _Initializations.GetProjectionMatrix();
                        }
                        mesh.Draw();
                    }
                }
                if (wall.WallType == WALLTYPE.WOOD2XFULLDMG)
                {
                    foreach (ModelMesh mesh in wallWood2xFullDmg.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;

                            effect.World = _Initializations.GetWorldMatrix();
                            effect.World *= Matrix.CreateRotationY(wall.WallRotation); //getting the rotation of the wall 
                            effect.World *= Matrix.CreateTranslation(wall.WallPosition);

                            worldMatrix = effect.World;

                            effect.View = cameraViewMatrix;
                            effect.Projection = _Initializations.GetProjectionMatrix();
                        }
                        mesh.Draw();
                    }
                }
                if (wall.WallType == WALLTYPE.WOOD2XHALFDMG)
                {
                    foreach (ModelMesh mesh in wallWood2xHalfDmg.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;

                            effect.World = _Initializations.GetWorldMatrix();
                            effect.World *= Matrix.CreateRotationY(wall.WallRotation); //getting the rotation of the wall 
                            effect.World *= Matrix.CreateTranslation(wall.WallPosition);

                            worldMatrix = effect.World;

                            effect.View = cameraViewMatrix;
                            effect.Projection = _Initializations.GetProjectionMatrix();
                        }
                        mesh.Draw();
                    }
                }
                if (wall.WallType == WALLTYPE.WOOD2XNODMG)
                {
                    foreach (ModelMesh mesh in wallWood2xNoDmg.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;

                            effect.World = _Initializations.GetWorldMatrix();
                            effect.World *= Matrix.CreateRotationY(wall.WallRotation); //getting the rotation of the wall 
                            effect.World *= Matrix.CreateTranslation(wall.WallPosition);

                            worldMatrix = effect.World;

                            effect.View = cameraViewMatrix;
                            effect.Projection = _Initializations.GetProjectionMatrix();
                        }
                        mesh.Draw();
                    }
                }
                if (wall.WallType == WALLTYPE.WOOD2XQUARTERDMG)
                {
                    foreach (ModelMesh mesh in wallWood2xQuarterDmg.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.PreferPerPixelLighting = true;

                            effect.World = _Initializations.GetWorldMatrix();
                            effect.World *= Matrix.CreateRotationY(wall.WallRotation); //getting the rotation of the wall 
                            effect.World *= Matrix.CreateTranslation(wall.WallPosition);

                            worldMatrix = effect.World;

                            effect.View = cameraViewMatrix;
                            effect.Projection = _Initializations.GetProjectionMatrix();
                        }
                        mesh.Draw();
                    }
                }
            }
            
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        //getters and setters
        
        public Matrix SetCameraMatrix
        {
            get { return cameraViewMatrix; }
            set { cameraViewMatrix = value; }
        }

        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
        }
        
        public List<Walls> BasementWalls
        {
            get { return basementWalls; }
            set { basementWalls = value; }
        }

        //for drawing bounding box visualizations
        public void drawBoundingBox(Walls walls)
        {
            //draw collison box for debugging
            //from here: https://electronicmeteor.wordpress.com/2011/10/25/bounding-boxes-for-your-model-meshes/
            // Initialize an array of indices for the box. 12 lines require 24 indices
            short[] bBoxIndices =
                {
                    0, 1, 1, 2, 2, 3, 3, 0, // Front edges
                    4, 5, 5, 6, 6, 7, 7, 4, // Back edges
                    0, 4, 1, 5, 2, 6, 3, 7 // Side edges connecting front and back
                };

            Vector3[] corners = walls.BoundingBox.GetCorners();
            VertexPositionColor[] primitiveList = new VertexPositionColor[corners.Length];

            // Assign the 8 box vertices
            for (int i = 0; i < corners.Length; i++)
            {
                primitiveList[i] = new VertexPositionColor(corners[i], Color.White);
            }

            /* Set your own effect parameters here */

            boxEffect.World = Matrix.Identity;
            //boxEffect.World *= Matrix.CreateFromAxisAngle(boxEffect.World.Up, walls.WallRotation);
            //boxEffect.World *= Matrix.CreateTranslation(walls.WallPosition);
            boxEffect.View = cameraViewMatrix;
            boxEffect.Projection = _Initializations.GetProjectionMatrix();
            boxEffect.TextureEnabled = false;

            // Draw the box with a LineList
            foreach (EffectPass pass in boxEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, primitiveList, 0, 8, bBoxIndices, 0, 12);
            }
        }
    }
}
