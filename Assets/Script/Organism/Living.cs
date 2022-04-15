using System.Collections;
using System.Collections.Generic;

public interface Living
{
    public delegate IEnumerator DamageAnimation();
    public DamageAnimation damageAnimation { set; }

    public delegate IEnumerator DeadAnimation();
    public DeadAnimation deadAnimation { set; }
}
