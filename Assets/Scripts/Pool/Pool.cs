using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//*************************
//创建人：Jaycr
//创建时间：#CreateTime#
//描述：对象池基类，使用泛型实现，可以用于任何类型的对象池，要求T必须是UnityEngine.Object的子类，这样才能在Unity中使用
//*************************

public class Pool<T> where T : UnityEngine.Object
{
    //T的对象池
    private readonly Stack<T> _poolStack = new Stack<T>();
    //创建对象的函数
    private readonly Func<T> _factory;
    //对象池的最大容量
    private int _maxSize;

    //构造函数，传入创建对象的函数和对象池的最大容量，最大容量默认为10
    public Pool(Func<T> func, int maxSize = 10)
    {
        this._factory = func;
        this._maxSize = maxSize;
        Debug.Log(_factory);
    }

    // 借出一个对象
    public T Rent()
    {
        return _poolStack.Count > 0 ? _poolStack.Pop() : _factory();
    }

    // 归还一个对象
    public void Return(T obj)
    {
        if (_poolStack.Count < _maxSize)
        {
            _poolStack.Push(obj);
        }
    }

    //扩大对象池的最大容量
    public void AddMaxSize(int size)
    {
        _maxSize += size;
    }
}
