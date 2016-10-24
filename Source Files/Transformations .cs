using UnityEngine;
using System.Collections;
using System;

public class Transformations : MonoBehaviour {

	// Use this for initialization
	void Start () {

        //original Cube
        Vector3[] cube = new Vector3[8];
        cube[0] = new Vector3(1, 1, 1);
        cube[1] = new Vector3(-1, 1, 1);
        cube[2] = new Vector3(-1, -1, 1);
        cube[3] = new Vector3(1, -1, 1);
        cube[4] = new Vector3(1, 1, -1);
        cube[5] = new Vector3(-1, 1, -1);
        cube[6] = new Vector3(-1, -1, -1);
        cube[7] = new Vector3(1, -1, -1);

        //transformation by rotation 37 degrees about the axis 11,-5,-5
        Vector3 startingAxis = new Vector3(11, -5, -5);
        startingAxis.Normalize();
        Quaternion rotation = Quaternion.AngleAxis(37, startingAxis);
        Matrix4x4 rotationMatrix =
            Matrix4x4.TRS(new Vector3(0,0,0),
                            rotation,
                            Vector3.one);
        //prints rotation matrx
        //printMatrix(rotationMatrix);

        Vector3[] newImage =
            MatrixTransform(cube, rotationMatrix);
        //prints cube after rotation
       // printVerts(newImage);


        //Scaling
        Matrix4x4 scaleMatrix =
            Matrix4x4.TRS(new Vector3(0,0,0),
                            Quaternion.identity,
                            new Vector3(11,2,-5));
        //prints scale matrix
        //printMatrix(scaleMatrix);

        Vector3[] scaleImage =
            MatrixTransform(newImage, scaleMatrix);
        //prints image after scaling
        //printVerts(scaleImage);

     
        //Translation
        Matrix4x4 translationMatrix =
            Matrix4x4.TRS(new Vector3(2, 3, 4),
                            Quaternion.identity,
                            Vector3.one);
        //prints translation matrix
       // printMatrix(translationMatrix);

        Vector3[] translationImage =
            MatrixTransform(scaleImage, translationMatrix);
        //prints tralslated image
        //printVerts(translationImage);



        //Single Matrix of transformations
        Matrix4x4 transformsMatrix = (translationMatrix * scaleMatrix)* rotationMatrix;

        //printMatrix(newMatrix);

        //Image after Transformations
        Vector3[] transformsImage = MatrixTransform(cube, transformsMatrix);

        //printVerts(transformsImage);
        Vector3 up = new Vector3(-4, -5, 11);
        up.Normalize();
        Vector3 forward = new Vector3(-5, 11, -5) - new Vector3(12, -2, 45);
        forward.Normalize();
        Quaternion q = Quaternion.LookRotation(forward, up);
        Vector3 translation = new Vector3(-13, 2, -45);

        //viewing matrix
        Matrix4x4 viewingMatrix = Matrix4x4.TRS(translation, q, Vector3.one);
        //printMatrix(viewingMatrix);

        //image after viewing matrix
        Vector3[] imageAfter = MatrixTransform(translationImage,viewingMatrix);
        //printVerts(imageAfter);


        Matrix4x4 projectionMulti = Matrix4x4.Perspective(90, 16 / 9, 1, 1000);

        Vector3[] finalCube = MatrixTransform(imageAfter, projectionMulti);
        //printVerts(finalCube);

        Matrix4x4 neoMatrix = (projectionMulti * viewingMatrix) * transformsMatrix;
        //printMatrix(neoMatrix);


        Vector3[] finalImage2 = MatrixTransform(cube, neoMatrix);
        printVerts(finalImage2);

        Vector2 mypoint1 = new Vector2(0.8f,1.5f);
        Vector2 mypoint2 = new Vector2(0.8f, 1.98f);
        Vector2 answer = intercept(mypoint1, mypoint2, 2);

        print(answer);
       


    }
    private void printVerts(Vector3[] newImage)
    {
        for (int i = 0; i < newImage.Length; i++)
            print(newImage[i].x + " , " +
                newImage[i].y + " , " +
                newImage[i].z);

    }

    private Vector3[] MatrixTransform(
        Vector3[] meshVertices, 
        Matrix4x4 transformMatrix)
    {
        Vector3[] output = new Vector3[meshVertices.Length];
        for (int i = 0; i < meshVertices.Length; i++)
            output[i] = transformMatrix * 
                new Vector4( 
                meshVertices[i].x,
                meshVertices[i].y,
                meshVertices[i].z,
                    1);

        return output;
    }

    private void printMatrix(Matrix4x4 matrix)
    {
        for (int i = 0; i < 4; i++)
            print(matrix.GetRow(i).ToString());
    }



    // Update is called once per frame
    void Update () {
	
	}

    public bool lineClip(ref Vector2 start, ref Vector2 finish)
    {
        Outcode oStart = new Outcode(start);
        Outcode oFinish = new Outcode(finish);

        if (oStart.isVisible() && oFinish.isVisible())
        {
            print("Trivally Accepted");
            return true;
        }

         if ((oStart & oFinish) != Outcode.zero() )
        {
            print("Trivally rejected");
            return false;

        }
        return false;
      /*   if(oStart.up) //clip start at top edge (y=1)
        {
             intercept(start, finish, 1);
        }
         if(oStart.down)
        {
            intercept(start, finish, 2);
        }
         if(oStart.left)
        {
            intercept(start, finish, 3);
        }
         if(oStart.right)
        {
            intercept(start, finish, 4);
        }
         if(oFinish.up)
        {
            intercept(finish, start, 1);
        }
         if(oFinish.down)
        {
            intercept(finish, start, 2);
        }
         if(oFinish.left)
        {
            intercept(finish, start, 3);
        }
         if(oFinish.right)
        {
            intercept(finish, start, 4);
        }*/

    } 

    public Vector2 intercept(Vector2 start, Vector2 finish,int edge)
    {
        float slope = (finish.y - start.y) / (finish.x - start.x);
        Vector2 answer = new Vector2();
        switch (edge)
        {
            
            case 1:
                answer.x = (1 / slope) * (1 - start.y) + start.x;
                answer.y = edge;
                break;
            case 2:
                answer.y = -1;
                answer.x = (1 / slope) * (-1 - start.y) + start.x;
                break;
            case 3:
                answer.x = -1;
                answer.y = start.y + (slope * (-1 - start.x));
                break;
            case 4:
                answer.x = 1;
                answer.y = start.y + (slope *( 1 - start.x));
                break;
        }

        return answer;

     

    }
}



public class Outcode 
{

    public bool up, down, left, right;
    Vector2 mypoint = new Vector2(-13, 12);
    Vector2 mypoint2 = new Vector2(2, 5);

    public Outcode()
    {


    }
    public Outcode(Vector2 point)
    {
        up = point.y > 1;
        down = point.y < -1;
        left = point.x < -1;
        right = point.x > 1;


    }

    public string display()
    {

        string output;
        if (up) output = "1"; else output = "0";
        if (down) output += "1"; else output += "0";
        if (left) output += "1"; else output += "0";
        if (right) output += "1"; else output += "0";

        return output;

    }

    public static bool operator ==(Outcode o1, Outcode o2)
    {
        return (o1.up && o2.up && o1.down && o2.down && o1.left && o2.left && o1.right && o2.right);

    }

    public static bool operator !=(Outcode o1, Outcode o2)
    {
        return (o1.up != o2.up) && (o1.down != o2.down) && (o1.left != o2.left) && (o1.right != o2.right);
    }

    public bool isVisible()
    {
        return (this == Outcode.zero());
    }


    public static Outcode zero()
    {
        return new Outcode(Vector2.zero);
    }

    public static Outcode operator  &(Outcode o1, Outcode o2)
    {
        Outcode answer = new Outcode();
        answer.up = o1.up && o2.up;
        answer.down = o2.down && o2.down;
        answer.left = o2.left && o2.left;
        answer.right = o1.right && o2.left;
        return answer;

    }


}





