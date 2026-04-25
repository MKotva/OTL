using Unity.VisualScripting;
using UnityEngine;
public class Id : MonoBehaviour
{
    
    [SerializeField] public int ident = 0;

    
    // Overload == operator
    public static bool operator ==(Id left, Id right)
    {
        // Both null or same reference
        if (ReferenceEquals(left, right))
            return true;
        
        // One is null
        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;
        
        // Compare by ident value
        return left.ident == right.ident;
    }
    
    // Overload != operator
    public static bool operator !=(Id left, Id right)
    {
        return !(left == right);
    }
    
    // Override Equals
    public override bool Equals(object obj)
    {
        Id other = obj as Id;
        if (other == null)
            return false;
        
        return this.ident == other.ident;
    }
    
    // Override GetHashCode
    public override int GetHashCode()
    {
        return ident.GetHashCode();
    }
}
