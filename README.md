# GloVe

C# implementation of GloVe algorithm ([Pennington et al., 2014](https://nlp.stanford.edu/projects/glove/))

#### Vector("king") - Vector("man") + Vector("woman") = Vector("queen")

### Special Notes

- For simplicity and clarity, training is done using vanilla stochastic gradient descent and not AdaGrad.
- Likewise, bias parameters are not included.

![J(ϴ)](J(ϴ).png)

```csharp
float sgd(Gram w, Gram c, float Pwc) {
	const int W = 0; const int C = VECTOR;

    float dot(float[] Vw, float[] Vc) {
        System.Diagnostics.Debug.Assert(Vw.Length == 2 * VECTOR);
        System.Diagnostics.Debug.Assert(Vc.Length == 2 * VECTOR);
        var y = 0f;
        for (int k = 0; k < VECTOR; k++) {
            y += Vw[k + W] * Vc[k + C];
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
        return (dot(w.Vector, c.Vector) - (float)Math.Log((double)x));
    }

    float ʝ = J(Pwc), ƒ = f(Pwc);

    const float α = 0.05f;

    for (int k = 0; k < VECTOR; k++) {
        float δJw = ƒ * ʝ * c.Vector[k + C];
        float δJc = ƒ * ʝ * w.Vector[k + W];
        w.Vector[k + W] -= α * δJw;
        c.Vector[k + C] -= α * δJc;
    }

    return ƒ * (ʝ * ʝ) / 2;
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
