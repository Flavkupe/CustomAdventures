using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class EffectTesterWindow : EditorWindow
{
    [MenuItem("Window/Utils/Effect Tester")]

    public static void ShowWindow()
    {
        GetWindow(typeof(EffectTesterWindow));
    }

    GameObject _source = null;
    GameObject _target = null;
    AnimationEffectData _effect = null;

    void OnGUI()
    {

        _source = EditorGUILayout.ObjectField("Source: ", _source, typeof(GameObject), true) as GameObject;
        _target = EditorGUILayout.ObjectField("Target: ", _target, typeof(GameObject), true) as GameObject;
        _effect = EditorGUILayout.ObjectField("Effect: ", _effect, typeof(AnimationEffectData), true) as AnimationEffectData;

        if (GUILayout.Button("Test"))
        {
            RunTest();
        }

        if (GUILayout.Button("Reset"))
        {
            Reset();
        }
    }

    private void RunTest()
    {
        if (_effect != null)
        {
            _currentEffect = _effect.CreateTargetedEffect(_target.transform.position, _source.transform.position);
            _currentEffect.runInEditMode = true;
            var routine = _currentEffect.CreateRoutine();
            routine.Finally(() => EditorApplication.update -= RunEffect);
            _current = routine;
            routine.DisableSafeMode = true;
            EditorApplication.update += RunEffect;
        }
    }

    private void Reset()
    {
        EditorApplication.update -= RunEffect;
    }

    IEnumerator _current = null;
    AnimationEffect _currentEffect = null;
    Stack<IEnumerator> _past = new Stack<IEnumerator>();

    private void RunEffect()
    {
        if (_current != null)
        {
            if (_current != _current.Current && _current.Current != null)
            {
                _past.Push(_current);
                _current = _current.Current as IEnumerator;
            }

            if (!_current.MoveNext())
            {
                _current = _past.Pop();
            }
        }
    }

    private void RunCoroutineRecursive(IEnumerator routine)
    {
        if (routine == null)
        {
            return;
        }

        if (routine.MoveNext())
        {
            Thread.Sleep(100);
            RunCoroutineRecursive(routine.Current as IEnumerator);
        }
    }
}

