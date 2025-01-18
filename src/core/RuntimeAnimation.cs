using System;
using System.Collections.Generic;

namespace Flatlanders.Core;

public class RuntimeAnimation
{
    public Animation Animation { get; }
    
    private List<RuntimeAnimationProperty> Properties { get; }

    public RuntimeAnimation(Animation animation)
    {
        Animation = animation;
        Properties = [];
    }

    public void BindProperty<T>(AnimationProperty<T> property, Action<T> valueChanged)
    {
        Properties.Add(new RuntimeAnimationProperty<T>(property, valueChanged));
    }

    public IEnumerable<RuntimeAnimationProperty> GetProperties()
    {
        return Properties;
    }
}
