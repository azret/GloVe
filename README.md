# GloVe

C# implementation of GloVe algorithm ([Pennington et al., 2014](https://nlp.stanford.edu/projects/glove/))

#### Vector("king") - Vector("man") + Vector("woman") = Vector("queen")

### Special Notes

- For simplicity and clarity, training is done using vanilla stochastic gradient descent and not AdaGrad.
- Likewise, bias parameters are not included.

![J(ϴ)](J(ϴ).png)

```csharp
 float sgd(Gram w, Gram c, float target) {

     float dot(float[] Vw, float[] Vc) {
         System.Diagnostics.Debug.Assert(Vw.Length == 2 * VECTOR);
         System.Diagnostics.Debug.Assert(Vc.Length == 2 * VECTOR);
         var y = 0f;
         for (int k = 0; k < VECTOR; k++) {
             y += Vw[k] * Vc[k + VECTOR];
         }
         return y;
     }

     float f(float x) {
         float y; float Xmax = 100f;
         if (x < Xmax) {
             y = (float)Math.Pow(x / Xmax, 0.75);
         } else {
             y = 1;
         }
         return y;
     }

     float J(float x) {
         return f(x) * (dot(w.Vector, c.Vector) - (float)Math.Log((double)x));
     }

     float ƒ = J(target);

     const float α = 0.05f;

     for (int k = 0; k < VECTOR; k++) {
         float δJw = ƒ * c.Vector[k + VECTOR];
         float δJc = ƒ * w.Vector[k];
         w.Vector[k] -= α * δJw;
         c.Vector[k + VECTOR] -= α * δJc;
     }

     return 0.5f * ƒ * ƒ;
 }
```

### Example

A small file (***data\en.vec***) with pre-trained vectors on select works of Cicero, Shakespeare and Leo Tolstoy's 
War and Peace is included.

```
W:\>king - man + woman

V('king') - V('man') + V('woman')

queen

W:\>father - man + woman

V('king') - V('man') + V('woman')

mother

W:\>
```
