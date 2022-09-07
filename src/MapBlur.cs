using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBlur{
    
    private float avg = 0;

    private float blurPixelCount = 0;
     
    public float[,] Blur(float[,] image, int radius, int iterations){
     
        float[,] tex = image;
     
        for (var i = 0; i < iterations; i++) {
     
            tex = BlurImage(tex, radius, true);
            tex = BlurImage(tex, radius, false);
        }
     
        return tex;
    }
     
    float[,] BlurImage(float[,] image, int blurSize, bool horizontal){
     
        float[,] blurred = new float[image.GetLength(0), image.GetLength(1)];
        int _W = image.GetLength(0);
        int _H = image.GetLength(1);
        int xx, yy, x, y;
     
        if (horizontal) {
     
            for (yy = 0; yy < _H; yy++) {
     
                for (xx = 0; xx < _W; xx++) { 
     
                    Reset();
     
                    //Right side of pixel
                    for (x = xx; x < xx + blurSize && x < _W; x++) {
     
                        AddValue(image[x, yy]);
                    }
     
                    //Left side of pixel
                    for (x = xx; x > xx - blurSize && x > 0; x--) {
     
                        AddValue(image[x, yy]);
                    }
     
                    CalcAverage();
     
                    for (x = xx; x < xx + blurSize && x < _W; x++) {
     
                        blurred[x, yy]=avg;
                    }
                }
            }
        }
     
        else {
     
            for (xx = 0; xx < _W; xx++) {
     
                for (yy = 0; yy < _H; yy++) {
     
                    Reset();
     
                    //Over pixel
                    for (y = yy; (y < yy + blurSize && y < _H); y++) {
     
                        AddValue(image[xx, y]);
                    }
     
                    //Under pixel
                    for (y = yy; (y > yy - blurSize && y > 0); y--) {
     
                        AddValue(image[xx, y]);
                    }
     
                    CalcAverage();
     
                    for (y = yy; y < yy + blurSize && y < _H; y++) {
     
                        blurred[xx, y]=avg;
                    }
                }
            }
        }
     
        return blurred;
    }
     
    void AddValue(float value) {
     
        avg+=value;
        blurPixelCount++;
    }
     
    void Reset() {
        avg = 0.0f;
        blurPixelCount = 0;
    }
     
    void CalcAverage() {
        avg = avg / blurPixelCount;
    }
}