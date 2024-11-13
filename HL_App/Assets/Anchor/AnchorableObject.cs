using System;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARSubsystems;

public class AnchorableObject : MonoBehaviour
{
    public bool LoadAnchorOnStart = true;

    [SerializeField, Tooltip("World anchors must have a unique name. Enter a descriptive name if you like.")]
    protected string _worldAnchorName;
    [SerializeField]
    protected TMPro.TMP_Text _debugLabel;

    [Space(10)]
    public UnityEvent OnAnchorLoaded;

    private TrackableId _xrAnchorId;
    private AnchorStoreManager _anchorStoreManager;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(_worldAnchorName))
        {
            _worldAnchorName = Guid.NewGuid().ToString();
        }
    }

    private void Start()
    {
        _anchorStoreManager = FindObjectOfType<AnchorStoreManager>();
        if (_anchorStoreManager)
        {
            _anchorStoreManager.PropertyChanged += AnchorStore_PropertyChanged;

            if (LoadAnchorOnStart && _anchorStoreManager.AnchorStore != null)
            {
                LoadAnchor();
            }
        }
        else
        {
            LogDebugMessage($"No {nameof(AnchorStoreManager)} present in scene.", true);
        }
    }

    private void LateUpdate()
    {
        UpdateAnchorPose();
    }

    public bool SaveAnchor(bool overwrite = true)
    {
        if (_anchorStoreManager?.AnchorPointsSubsystem == null || _anchorStoreManager?.AnchorStore == null)
        {
            LogDebugMessage($"Can't save anchor {_worldAnchorName}: reference point subsystem or anchor store have not been initialized.", true);
            return false;
        }

        XRAnchor anchor;

        if (overwrite)
        {
            // Delete the current anchor in the store, so we can persist the new position.
            _anchorStoreManager.AnchorStore.UnpersistAnchor(_worldAnchorName);
        }

        // Attempt to save the anchor.
        if (_anchorStoreManager.AnchorPointsSubsystem.TryAddAnchor(new Pose(transform.position, transform.rotation), out anchor))
        {
            if (_anchorStoreManager.AnchorStore.TryPersistAnchor(anchor.trackableId, _worldAnchorName))
            {
                _xrAnchorId = anchor.trackableId;
                LogDebugMessage($"Successfully saved anchor {_worldAnchorName}.");
                PositionFromAnchor(anchor);
                return true;
            }
            else
            {
                LogDebugMessage($"Failed to save anchor {_worldAnchorName}.", true);
            }
        }
        else
        {
            LogDebugMessage($"Failed to add reference point for anchor {_worldAnchorName}.", true);
        }

        return false;
    }

    public bool LoadAnchor()
    {
        if (_anchorStoreManager?.AnchorPointsSubsystem == null || _anchorStoreManager?.AnchorStore == null)
        {
            LogDebugMessage($"Can't load anchor {_worldAnchorName}: reference point subsystem or anchor store have not been initialized.", true);
            return false;
        }

        // Retrieve the trackable id from the anchor store.
        TrackableId trackableId = _anchorStoreManager.AnchorStore.LoadAnchor(_worldAnchorName);

        // Look for the matching anchor in the anchor point subsystem.
        TrackableChanges<XRAnchor> referencePointChanges = _anchorStoreManager.AnchorPointsSubsystem.GetChanges(Allocator.Temp);
        foreach (XRAnchor anchor in referencePointChanges.added)
        {
            if (anchor.trackableId == trackableId)
            {
                _xrAnchorId = anchor.trackableId;
                PositionFromAnchor(anchor);
                OnAnchorLoaded.Invoke();
                LogDebugMessage($"Found anchor {_worldAnchorName} in added reference points.");
                return true;
            }
        }

        LogDebugMessage($"Did not find anchor {_worldAnchorName} in reference points subsystem after XRAnchorStore load.", true);
        return false;
    }

    public void ClearAnchor()
    {
        _xrAnchorId = default;

        if (_anchorStoreManager.AnchorStore != null)
        {
            _anchorStoreManager.AnchorStore.UnpersistAnchor(_worldAnchorName);
        }
    }

    private void UpdateAnchorPose()
    {
        if (_xrAnchorId == default || _anchorStoreManager?.AnchorPointsSubsystem == null)
        {
            return;
        }

        TrackableChanges<XRAnchor> anchorChanges = _anchorStoreManager.AnchorPointsSubsystem.GetChanges(Allocator.Temp);

        foreach (XRAnchor anchor in anchorChanges.updated)
        {
            if (anchor.trackableId == _xrAnchorId)
            {
                PositionFromAnchor(anchor);
                break;
            }
        }
    }

    private void PositionFromAnchor(XRAnchor anchor)
    {
        if (_debugLabel)
        {
            _debugLabel.text = $"{_worldAnchorName} tracking: {anchor.trackingState}\r\n{anchor.pose.position}\r\n{anchor.pose.rotation}";
        }

        transform.position = anchor.pose.position;
        transform.rotation = anchor.pose.rotation;
    }

    private void AnchorStore_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (LoadAnchorOnStart && e.PropertyName == nameof(AnchorStoreManager.AnchorStore) && _anchorStoreManager.AnchorStore != null)
        {
            LoadAnchor();
        }
    }

    private void LogDebugMessage(string message, bool isWarning = false)
    {
        if (_debugLabel)
        {
            _debugLabel.text = message;
        }

        if (isWarning)
        {
            Debug.LogWarning($"[{GetType()}] {message}");
        }
        else
        {
            Debug.Log($"[{GetType()}] {message}");
        }
    }
}