using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamelayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask interactableLayer;
    [SerializeField] LayerMask longGrassLayer;
    [SerializeField] LayerMask southSlope;
    [SerializeField] LayerMask westSlope;
    [SerializeField] LayerMask northSlope;
    [SerializeField] LayerMask eastSlope;
    [SerializeField] LayerMask pikachuLayer;
    [SerializeField] LayerMask fovLayer;
    [SerializeField] LayerMask portalLayer;


    public static Gamelayers i { get; set;}

    private void Awake()
    {
        i = this;
    }


    public LayerMask SolidLayer
    {
        get => solidObjectsLayer;
    }
    public LayerMask PlayerLayer
    {
        get => playerLayer;
    }
    public LayerMask InteractableLayer
    {
        get => interactableLayer;
    }
    public LayerMask LongGrassLayer
    {
        get => longGrassLayer;
    }
    public LayerMask SouthSlopeLayer
    {
        get => southSlope;
    }
    public LayerMask NorthSlopeLayer
    {
        get => northSlope;
    }
    public LayerMask WestSlopeLayer
    {
        get => westSlope;
    }
    public LayerMask EastSlopeLayer
    {
        get => eastSlope;
    }
    public LayerMask PikachuLayer
    {
        get => pikachuLayer;
    }
    public LayerMask FOVLayer
    {
        get => fovLayer;
    }
    public LayerMask PortalLayer
    {
        get => portalLayer;
    }
    public LayerMask TriggerableLayers
    {
        get => longGrassLayer | fovLayer | portalLayer;
    }
}
