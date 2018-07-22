namespace Invaders.Wpf.Model
{
    ///<summary>
    ///Instead of interface <see cref="System.ICloneable" /> , <see cref="Invaders.Wpf.Model.ICloneable{T}" /> let
    /// users do less work to check object type.
    ///</summary>
    interface ICloneable<T>
    {
        T Clone();
    }
}