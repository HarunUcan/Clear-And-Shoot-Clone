using UnityEngine;

public interface ICollectable
{
    public bool IsStartPointPainted { get; set; }
    public bool IsEndPointPainted { get; set; }
    public bool IsCleaned { get; set; }
    public float CleanPercentThreshold { get; set; }
    public void Collect(Transform target);
}
