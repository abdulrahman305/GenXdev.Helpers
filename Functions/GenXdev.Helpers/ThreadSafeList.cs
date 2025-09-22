// ################################################################################
// Part of PowerShell module : GenXdev.Helpers
// Original cmdlet filename  : ThreadSafeList.cs
// Original author           : René Vaessen / GenXdev
// Version                   : 1.280.2025
// ################################################################################
// MIT License
//
// Copyright 2021-2025 GenXdev
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// ################################################################################



using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime;

namespace GenXdev.Containers
{

    // Summary:
    //     Represents a strongly typed list of objects that can be accessed by index.
    //     Provides methods to search, sort, and manipulate lists.
    //
    // Type parameters:
    //   T:
    //     The type of elements in the list.
    [Serializable]
    [DebuggerDisplay("Count = {Count}")]
    public class ThreadSafeList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable
    {
        /// <summary>
        /// Creates a thread-safe snapshot of the current list contents as an array.
        /// </summary>
        /// <returns>Array copy of all current elements.</returns>
        public T[] ToThreadSafeEnumerable()
        {
            // thread-safe snapshot by copying under lock
            lock (padLock)
            {
                return internalList.ToArray<T>();
            }
        }

        /// <summary>
        /// Delegate for matching items during update operations.
        /// </summary>
        /// <param name="item1">First item to compare.</param>
        /// <param name="item2">Second item to compare.</param>
        /// <returns>True if items match for update purposes.</returns>
        public delegate bool UpdateCallbackMatch(T item1, T item2);

        /// <summary>
        /// Delegate for updating existing items with new values.
        /// </summary>
        /// <param name="itemToUpdate">The existing item to modify.</param>
        /// <param name="newValues">The new values to apply.</param>
        public delegate void UpdateCallbackUpdate(T itemToUpdate, T newValues);

        /// <summary>
        /// Synchronization object for thread-safe operations.
        /// </summary>
        object padLock = new object();

        /// <summary>
        /// Internal list that stores the actual data.
        /// </summary>
        List<T> internalList;

        // Summary:
        //     Initializes a new instance of the GenXdev.Containers.ThreadSafeList<T> class
        //     that is empty and has the default initial capacity.
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public ThreadSafeList()
        {
            internalList = new List<T>();
        }
        //
        // Summary:
        //     Initializes a new instance of the GenXdev.Containers.ThreadSafeList<T> class
        //     that contains elements copied from the specified collection and has sufficient
        //     capacity to accommodate the number of elements copied.
        //
        // Parameters:
        //   collection:
        //     The collection whose elements are copied to the new list.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     collection is null.
        public ThreadSafeList(IEnumerable<T> collection)
        {
            internalList = new List<T>(collection);
        }
        //
        // Summary:
        //     Initializes a new instance of the GenXdev.Containers.ThreadSafeList<T> class
        //     that is empty and has the specified initial capacity.
        //
        // Parameters:
        //   capacity:
        //     The number of elements that the new list can initially store.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     capacity is less than 0.
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public ThreadSafeList(int capacity)
        {
            internalList = new List<T>(capacity);
        }

        // Summary:
        //     Gets or sets the total number of elements the internal data structure can
        //     hold without resizing.
        //
        // Returns:
        //     The number of elements that the GenXdev.Containers.ThreadSafeList<T> can contain
        //     before resizing is required.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     GenXdev.Containers.ThreadSafeList<T>.Capacity is set to a value that is less
        //     than GenXdev.Containers.ThreadSafeList<T>.Count.
        //
        //   System.OutOfMemoryException:
        //     There is not enough memory available on the system.
        public int Capacity
        {
            get
            {
                lock (padLock)
                {
                    return internalList.Capacity;
                }
            }
            set
            {
                lock (padLock)
                {
                    internalList.Capacity = value;
                }
            }
        }
        //
        // Summary:
        //     Gets the number of elements actually contained in the GenXdev.Containers.ThreadSafeList<T>.
        //
        // Returns:
        //     The number of elements actually contained in the GenXdev.Containers.ThreadSafeList<T>.
        public int Count
        {
            get
            {
                lock (padLock)
                {
                    return internalList.Count;
                }
            }
        }

        // Summary:
        //     Gets or sets the element at the specified index.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the element to get or set.
        //
        // Returns:
        //     The element at the specified index.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is less than 0.-or-index is equal to or greater than GenXdev.Containers.ThreadSafeList<T>.Count.
        public T this[int index]
        {
            get
            {
                lock (padLock)
                {
                    return internalList[index];
                }
            }
            set
            {
                lock (padLock)
                {
                    internalList[index] = value;
                }
            }
        }

        // Summary:
        //     Adds an object to the end of the GenXdev.Containers.ThreadSafeList<T>.
        //
        // Parameters:
        //   item:
        //     The object to be added to the end of the GenXdev.Containers.ThreadSafeList<T>.
        //     The value can be null for reference types.
        public void Add(T item)
        {
            lock (padLock)
            {
                internalList.Add(item);
            }
        }
        //
        // Summary:
        //     Adds the elements of the specified collection to the end of the GenXdev.Containers.ThreadSafeList<T>.
        //
        // Parameters:
        //   collection:
        //     The collection whose elements should be added to the end of the GenXdev.Containers.ThreadSafeList<T>.
        //     The collection itself cannot be null, but it can contain elements that are
        //     null, if type T is a reference type.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     collection is null.
        public void AddRange(IEnumerable<T> collection)
        {
            lock (padLock)
            {
                internalList.AddRange(collection);
            }
        }
        //
        // Summary:
        //     Returns a read-only System.Collections.Generic.IList<T> wrapper for the current
        //     collection.
        //
        // Returns:
        //     A System.Collections.ObjectModel.ReadOnlyCollection<T> that acts as a read-only
        //     wrapper around the current GenXdev.Containers.ThreadSafeList<T>.
        public ReadOnlyCollection<T> AsReadOnly()
        {
            lock (padLock)
            {
                return internalList.AsReadOnly();
            }
        }
        //
        // Summary:
        //     Searches the entire sorted GenXdev.Containers.ThreadSafeList<T> for an element
        //     using the default comparer and returns the zero-based index of the element.
        //
        // Parameters:
        //   item:
        //     The object to locate. The value can be null for reference types.
        //
        // Returns:
        //     The zero-based index of item in the sorted GenXdev.Containers.ThreadSafeList<T>,
        //     if item is found; otherwise, a negative number that is the bitwise complement
        //     of the index of the next element that is larger than item or, if there is
        //     no larger element, the bitwise complement of GenXdev.Containers.ThreadSafeList<T>.Count.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The default comparer System.Collections.Generic.Comparer<T>.Default cannot
        //     find an implementation of the System.IComparable<T> generic interface or
        //     the System.IComparable interface for type T.
        public int BinarySearch(T item)
        {
            lock (padLock)
            {
                return internalList.BinarySearch(item);
            }
        }
        //
        // Summary:
        //     Searches the entire sorted GenXdev.Containers.ThreadSafeList<T> for an element
        //     using the specified comparer and returns the zero-based index of the element.
        //
        // Parameters:
        //   item:
        //     The object to locate. The value can be null for reference types.
        //
        //   comparer:
        //     The System.Collections.Generic.IComparer<T> implementation to use when comparing
        //     elements.-or-null to use the default comparer System.Collections.Generic.Comparer<T>.Default.
        //
        // Returns:
        //     The zero-based index of item in the sorted GenXdev.Containers.ThreadSafeList<T>,
        //     if item is found; otherwise, a negative number that is the bitwise complement
        //     of the index of the next element that is larger than item or, if there is
        //     no larger element, the bitwise complement of GenXdev.Containers.ThreadSafeList<T>.Count.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     comparer is null, and the default comparer System.Collections.Generic.Comparer<T>.Default
        //     cannot find an implementation of the System.IComparable<T> generic interface
        //     or the System.IComparable interface for type T.
        public int BinarySearch(T item, IComparer<T> comparer)
        {
            lock (padLock)
            {
                return internalList.BinarySearch(item, comparer);
            }
        }
        //
        // Summary:
        //     Searches a range of elements in the sorted GenXdev.Containers.ThreadSafeList<T>
        //     for an element using the specified comparer and returns the zero-based index
        //     of the element.
        //
        // Parameters:
        //   index:
        //     The zero-based starting index of the range to search.
        //
        //   count:
        //     The length of the range to search.
        //
        //   item:
        //     The object to locate. The value can be null for reference types.
        //
        //   comparer:
        //     The System.Collections.Generic.IComparer<T> implementation to use when comparing
        //     elements, or null to use the default comparer System.Collections.Generic.Comparer<T>.Default.
        //
        // Returns:
        //     The zero-based index of item in the sorted GenXdev.Containers.ThreadSafeList<T>,
        //     if item is found; otherwise, a negative number that is the bitwise complement
        //     of the index of the next element that is larger than item or, if there is
        //     no larger element, the bitwise complement of GenXdev.Containers.ThreadSafeList<T>.Count.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is less than 0.-or-count is less than 0.
        //
        //   System.ArgumentException:
        //     index and count do not denote a valid range in the GenXdev.Containers.ThreadSafeList<T>.
        //
        //   System.InvalidOperationException:
        //     comparer is null, and the default comparer System.Collections.Generic.Comparer<T>.Default
        //     cannot find an implementation of the System.IComparable<T> generic interface
        //     or the System.IComparable interface for type T.
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            lock (padLock)
            {
                return internalList.BinarySearch(index, count, item, comparer);
            }
        }
        //
        // Summary:
        //     Removes all elements from the GenXdev.Containers.ThreadSafeList<T>.
        public void Clear()
        {
            lock (padLock)
            {
                internalList.Clear();
            }
        }
        //
        // Summary:
        //     Determines whether an element is in the GenXdev.Containers.ThreadSafeList<T>.
        //
        // Parameters:
        //   item:
        //     The object to locate in the GenXdev.Containers.ThreadSafeList<T>. The value
        //     can be null for reference types.
        //
        // Returns:
        //     true if item is found in the GenXdev.Containers.ThreadSafeList<T>; otherwise,
        //     false.
        public bool Contains(T item)
        {
            lock (padLock)
            {
                return internalList.Contains(item);
            }
        }
        //
        // Summary:
        //     Converts the elements in the current GenXdev.Containers.ThreadSafeList<T> to
        //     another type, and returns a list containing the converted elements.
        //
        // Parameters:
        //   converter:
        //     A System.Converter<TInput,TOutput> delegate that converts each element from
        //     one type to another type.
        //
        // Type parameters:
        //   TOutput:
        //     The type of the elements of the target array.
        //
        // Returns:
        //     A GenXdev.Containers.ThreadSafeList<T> of the target type containing the converted
        //     elements from the current GenXdev.Containers.ThreadSafeList<T>.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     converter is null.
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            lock (padLock)
            {
                return internalList.ConvertAll<TOutput>(converter);
            }
        }
        //
        // Summary:
        //     Copies the entire GenXdev.Containers.ThreadSafeList<T> to a compatible one-dimensional
        //     array, starting at the beginning of the target array.
        //
        // Parameters:
        //   array:
        //     The one-dimensional System.Array that is the destination of the elements
        //     copied from GenXdev.Containers.ThreadSafeList<T>. The System.Array must have
        //     zero-based indexing.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     array is null.
        //
        //   System.ArgumentException:
        //     The number of elements in the source GenXdev.Containers.ThreadSafeList<T> is
        //     greater than the number of elements that the destination array can contain.
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public void CopyTo(T[] array)
        {
            lock (padLock)
            {
                internalList.CopyTo(array);
            }
        }
        //
        // Summary:
        //     Copies the entire GenXdev.Containers.ThreadSafeList<T> to a compatible one-dimensional
        //     array, starting at the specified index of the target array.
        //
        // Parameters:
        //   array:
        //     The one-dimensional System.Array that is the destination of the elements
        //     copied from GenXdev.Containers.ThreadSafeList<T>. The System.Array must have
        //     zero-based indexing.
        //
        //   arrayIndex:
        //     The zero-based index in array at which copying begins.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     array is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     arrayIndex is less than 0.
        //
        //   System.ArgumentException:
        //     The number of elements in the source GenXdev.Containers.ThreadSafeList<T> is
        //     greater than the available space from arrayIndex to the end of the destination
        //     array.
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (padLock)
            {
                internalList.CopyTo(array, arrayIndex);
            }
        }
        //
        // Summary:
        //     Copies a range of elements from the GenXdev.Containers.ThreadSafeList<T> to
        //     a compatible one-dimensional array, starting at the specified index of the
        //     target array.
        //
        // Parameters:
        //   index:
        //     The zero-based index in the source GenXdev.Containers.ThreadSafeList<T> at
        //     which copying begins.
        //
        //   array:
        //     The one-dimensional System.Array that is the destination of the elements
        //     copied from GenXdev.Containers.ThreadSafeList<T>. The System.Array must have
        //     zero-based indexing.
        //
        //   arrayIndex:
        //     The zero-based index in array at which copying begins.
        //
        //   count:
        //     The number of elements to copy.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     array is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     index is less than 0.-or-arrayIndex is less than 0.-or-count is less than
        //     0.
        //
        //   System.ArgumentException:
        //     index is equal to or greater than the GenXdev.Containers.ThreadSafeList<T>.Count
        //     of the source GenXdev.Containers.ThreadSafeList<T>.-or-The number of elements
        //     from index to the end of the source GenXdev.Containers.ThreadSafeList<T> is
        //     greater than the available space from arrayIndex to the end of the destination
        //     array.
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            lock (padLock)
            {
                internalList.CopyTo(index, array, arrayIndex, count);
            }
        }
        //
        // Summary:
        //     Determines whether the GenXdev.Containers.ThreadSafeList<T> contains elements
        //     that match the conditions defined by the specified predicate.
        //
        // Parameters:
        //   match:
        //     The System.Predicate<T> delegate that defines the conditions of the elements
        //     to search for.
        //
        // Returns:
        //     true if the GenXdev.Containers.ThreadSafeList<T> contains one or more elements
        //     that match the conditions defined by the specified predicate; otherwise,
        //     false.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     match is null.
        public bool Exists(Predicate<T> match)
        {
            lock (padLock)
            {
                return internalList.Exists(match);
            }
        }
        //
        // Summary:
        //     Searches for an element that matches the conditions defined by the specified
        //     predicate, and returns the first occurrence within the entire GenXdev.Containers.ThreadSafeList<T>.
        //
        // Parameters:
        //   match:
        //     The System.Predicate<T> delegate that defines the conditions of the element
        //     to search for.
        //
        // Returns:
        //     The first element that matches the conditions defined by the specified predicate,
        //     if found; otherwise, the default value for type T.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     match is null.
        public T Find(Predicate<T> match)
        {
            lock (padLock)
            {
                return internalList.Find(match);
            }
        }

        //
        // Summary:
        //     Retrieves all the elements that match the conditions defined by the specified
        //     predicate.
        //
        // Parameters:
        //   match:
        //     The System.Predicate<T> delegate that defines the conditions of the elements
        //     to search for.
        //
        // Returns:
        //     A GenXdev.Containers.ThreadSafeList<T> containing all the elements that match
        //     the conditions defined by the specified predicate, if found; otherwise, an
        //     empty GenXdev.Containers.ThreadSafeList<T>.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     match is null.
        public List<T> FindAll(Predicate<T> match)
        {
            lock (padLock)
            {
                return internalList.FindAll(match);
            }
        }
        //
        // Summary:
        //     Searches for an element that matches the conditions defined by the specified
        //     predicate, and returns the zero-based index of the first occurrence within
        //     the entire GenXdev.Containers.ThreadSafeList<T>.
        //
        // Parameters:
        //   match:
        //     The System.Predicate<T> delegate that defines the conditions of the element
        //     to search for.
        //
        // Returns:
        //     The zero-based index of the first occurrence of an element that matches the
        //     conditions defined by match, if found; otherwise, –1.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     match is null.
        public int FindIndex(Predicate<T> match)
        {
            lock (padLock)
            {
                return internalList.FindIndex(match);
            }
        }
        //
        // Summary:
        //     Searches for an element that matches the conditions defined by the specified
        //     predicate, and returns the zero-based index of the first occurrence within
        //     the range of elements in the GenXdev.Containers.ThreadSafeList<T> that extends
        //     from the specified index to the last element.
        //
        // Parameters:
        //   startIndex:
        //     The zero-based starting index of the search.
        //
        //   match:
        //     The System.Predicate<T> delegate that defines the conditions of the element
        //     to search for.
        //
        // Returns:
        //     The zero-based index of the first occurrence of an element that matches the
        //     conditions defined by match, if found; otherwise, –1.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     match is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     startIndex is outside the range of valid indexes for the GenXdev.Containers.ThreadSafeList<T>.
        public int FindIndex(int startIndex, Predicate<T> match)
        {
            lock (padLock)
            {
                return internalList.FindIndex(startIndex, match);
            }
        }
        //
        // Summary:
        //     Searches for an element that matches the conditions defined by the specified
        //     predicate, and returns the zero-based index of the first occurrence within
        //     the range of elements in the GenXdev.Containers.ThreadSafeList<T> that starts
        //     at the specified index and contains the specified number of elements.
        //
        // Parameters:
        //   startIndex:
        //     The zero-based starting index of the search.
        //
        //   count:
        //     The number of elements in the section to search.
        //
        //   match:
        //     The System.Predicate<T> delegate that defines the conditions of the element
        //     to search for.
        //
        // Returns:
        //     The zero-based index of the first occurrence of an element that matches the
        //     conditions defined by match, if found; otherwise, –1.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     match is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     startIndex is outside the range of valid indexes for the GenXdev.Containers.ThreadSafeList<T>.-or-count
        //     is less than 0.-or-startIndex and count do not specify a valid section in
        //     the GenXdev.Containers.ThreadSafeList<T>.
        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            lock (padLock)
            {
                return internalList.FindIndex(startIndex, count, match);
            }
        }
        //
        // Summary:
        //     Searches for an element that matches the conditions defined by the specified
        //     predicate, and returns the last occurrence within the entire GenXdev.Containers.ThreadSafeList<T>.
        //
        // Parameters:
        //   match:
        //     The System.Predicate<T> delegate that defines the conditions of the element
        //     to search for.
        //
        // Returns:
        //     The last element that matches the conditions defined by the specified predicate,
        //     if found; otherwise, the default value for type T.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     match is null.
        public T FindLast(Predicate<T> match)
        {
            lock (padLock)
            {
                return internalList.FindLast(match);
            }
        }
        //
        // Summary:
        //     Searches for an element that matches the conditions defined by the specified
        //     predicate, and returns the zero-based index of the last occurrence within
        //     the entire GenXdev.Containers.ThreadSafeList<T>.
        //
        // Parameters:
        //   match:
        //     The System.Predicate<T> delegate that defines the conditions of the element
        //     to search for.
        //
        // Returns:
        //     The zero-based index of the last occurrence of an element that matches the
        //     conditions defined by match, if found; otherwise, –1.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     match is null.
        public int FindLastIndex(Predicate<T> match)
        {
            lock (padLock)
            {
                return internalList.FindLastIndex(match);
            }
        }
        //
        // Summary:
        //     Searches for an element that matches the conditions defined by the specified
        //     predicate, and returns the zero-based index of the last occurrence within
        //     the range of elements in the GenXdev.Containers.ThreadSafeList<T> that extends
        //     from the first element to the specified index.
        //
        // Parameters:
        //   startIndex:
        //     The zero-based starting index of the backward search.
        //
        //   match:
        //     The System.Predicate<T> delegate that defines the conditions of the element
        //     to search for.
        //
        // Returns:
        //     The zero-based index of the last occurrence of an element that matches the
        //     conditions defined by match, if found; otherwise, –1.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     match is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     startIndex is outside the range of valid indexes for the GenXdev.Containers.ThreadSafeList<T>.
        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            lock (padLock)
            {
                return internalList.FindLastIndex(startIndex, match);
            }
        }
        //
        // Summary:
        //     Searches for an element that matches the conditions defined by the specified
        //     predicate, and returns the zero-based index of the last occurrence within
        //     the range of elements in the GenXdev.Containers.ThreadSafeList<T> that contains
        //     the specified number of elements and ends at the specified index.
        //
        // Parameters:
        //   startIndex:
        //     The zero-based starting index of the backward search.
        //
        //   count:
        //     The number of elements in the section to search.
        //
        //   match:
        //     The System.Predicate<T> delegate that defines the conditions of the element
        //     to search for.
        //
        // Returns:
        //     The zero-based index of the last occurrence of an element that matches the
        //     conditions defined by match, if found; otherwise, –1.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     match is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     startIndex is outside the range of valid indexes for the GenXdev.Containers.ThreadSafeList<T>.-or-count
        //     is less than 0.-or-startIndex and count do not specify a valid section in
        //     the GenXdev.Containers.ThreadSafeList<T>.
        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            lock (padLock)
            {
                return internalList.FindLastIndex(startIndex, count, match);
            }
        }
        //
        // Summary:
        //     Performs the specified action on each element of the GenXdev.Containers.ThreadSafeList<T>.
        //
        // Parameters:
        //   action:
        //     The System.Action<T> delegate to perform on each element of the GenXdev.Containers.ThreadSafeList<T>.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     action is null.
        public void ForEach(Action<T> action)
        {
            lock (padLock)
            {
                internalList.ForEach(action);
            }
        }
        //
        // Summary:
        //     Returns an enumerator that iterates through the GenXdev.Containers.ThreadSafeList<T>.
        //
        // Returns:
        //     A GenXdev.Containers.ThreadSafeList<T>.Enumerator for the GenXdev.Containers.ThreadSafeList<T>.
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public List<T>.Enumerator GetEnumerator()
        {
            lock (padLock)
            {
                return internalList.GetEnumerator();
            }
        }
        //
        // Summary:
        //     Creates a shallow copy of a range of elements in the source GenXdev.Containers.ThreadSafeList<T>.
        //
        // Parameters:
        //   index:
        //     The zero-based GenXdev.Containers.ThreadSafeList<T> index at which the range
        //     starts.
        //
        //   count:
        //     The number of elements in the range.
        //
        // Returns:
        //     A shallow copy of a range of elements in the source GenXdev.Containers.ThreadSafeList<T>.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is less than 0.-or-count is less than 0.
        //
        //   System.ArgumentException:
        //     index and count do not denote a valid range of elements in the GenXdev.Containers.ThreadSafeList<T>.
        public List<T> GetRange(int index, int count)
        {
            lock (padLock)
            {
                return internalList.GetRange(index, count);
            }
        }
        //
        // Summary:
        //     Searches for the specified object and returns the zero-based index of the
        //     first occurrence within the entire GenXdev.Containers.ThreadSafeList<T>.
        //
        // Parameters:
        //   item:
        //     The object to locate in the GenXdev.Containers.ThreadSafeList<T>. The value
        //     can be null for reference types.
        //
        // Returns:
        //     The zero-based index of the first occurrence of item within the entire GenXdev.Containers.ThreadSafeList<T>,
        //     if found; otherwise, –1.
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public int IndexOf(T item)
        {
            lock (padLock)
            {
                return internalList.IndexOf(item);
            }
        }
        //
        // Summary:
        //     Searches for the specified object and returns the zero-based index of the
        //     first occurrence within the range of elements in the GenXdev.Containers.ThreadSafeList<T>
        //     that extends from the specified index to the last element.
        //
        // Parameters:
        //   item:
        //     The object to locate in the GenXdev.Containers.ThreadSafeList<T>. The value
        //     can be null for reference types.
        //
        //   index:
        //     The zero-based starting index of the search. 0 (zero) is valid in an empty
        //     list.
        //
        // Returns:
        //     The zero-based index of the first occurrence of item within the range of
        //     elements in the GenXdev.Containers.ThreadSafeList<T> that extends from index
        //     to the last element, if found; otherwise, –1.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is outside the range of valid indexes for the GenXdev.Containers.ThreadSafeList<T>.
        public int IndexOf(T item, int index)
        {
            lock (padLock)
            {
                return internalList.IndexOf(item, index);
            }
        }
        //
        // Summary:
        //     Searches for the specified object and returns the zero-based index of the
        //     first occurrence within the range of elements in the GenXdev.Containers.ThreadSafeList<T>
        //     that starts at the specified index and contains the specified number of elements.
        //
        // Parameters:
        //   item:
        //     The object to locate in the GenXdev.Containers.ThreadSafeList<T>. The value
        //     can be null for reference types.
        //
        //   index:
        //     The zero-based starting index of the search. 0 (zero) is valid in an empty
        //     list.
        //
        //   count:
        //     The number of elements in the section to search.
        //
        // Returns:
        //     The zero-based index of the first occurrence of item within the range of
        //     elements in the GenXdev.Containers.ThreadSafeList<T> that starts at index and
        //     contains count number of elements, if found; otherwise, –1.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is outside the range of valid indexes for the GenXdev.Containers.ThreadSafeList<T>.-or-count
        //     is less than 0.-or-index and count do not specify a valid section in the
        //     GenXdev.Containers.ThreadSafeList<T>.
        public int IndexOf(T item, int index, int count)
        {
            lock (padLock)
            {
                return internalList.IndexOf(item, index, count);
            }
        }
        //
        // Summary:
        //     Inserts an element into the GenXdev.Containers.ThreadSafeList<T> at the specified
        //     index.
        //
        // Parameters:
        //   index:
        //     The zero-based index at which item should be inserted.
        //
        //   item:
        //     The object to insert. The value can be null for reference types.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is less than 0.-or-index is greater than GenXdev.Containers.ThreadSafeList<T>.Count.
        public void Insert(int index, T item)
        {
            lock (padLock)
            {
                internalList.Insert(index, item);
            }
        }
        //
        // Summary:
        //     Inserts the elements of a collection into the GenXdev.Containers.ThreadSafeList<T>
        //     at the specified index.
        //
        // Parameters:
        //   index:
        //     The zero-based index at which the new elements should be inserted.
        //
        //   collection:
        //     The collection whose elements should be inserted into the GenXdev.Containers.ThreadSafeList<T>.
        //     The collection itself cannot be null, but it can contain elements that are
        //     null, if type T is a reference type.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     collection is null.
        //
        //   System.ArgumentOutOfRangeException:
        //     index is less than 0.-or-index is greater than GenXdev.Containers.ThreadSafeList<T>.Count.
        public void InsertRange(int index, IEnumerable<T> collection)
        {
            lock (padLock)
            {
                internalList.InsertRange(index, collection);
            }
        }
        //
        // Summary:
        //     Searches for the specified object and returns the zero-based index of the
        //     last occurrence within the entire GenXdev.Containers.ThreadSafeList<T>.
        //
        // Parameters:
        //   item:
        //     The object to locate in the GenXdev.Containers.ThreadSafeList<T>. The value
        //     can be null for reference types.
        //
        // Returns:
        //     The zero-based index of the last occurrence of item within the entire the
        //     GenXdev.Containers.ThreadSafeList<T>, if found; otherwise, –1.
        public int LastIndexOf(T item)
        {
            lock (padLock)
            {
                return internalList.LastIndexOf(item);
            }
        }
        //
        // Summary:
        //     Searches for the specified object and returns the zero-based index of the
        //     last occurrence within the range of elements in the GenXdev.Containers.ThreadSafeList<T>
        //     that extends from the first element to the specified index.
        //
        // Parameters:
        //   item:
        //     The object to locate in the GenXdev.Containers.ThreadSafeList<T>. The value
        //     can be null for reference types.
        //
        //   index:
        //     The zero-based starting index of the backward search.
        //
        // Returns:
        //     The zero-based index of the last occurrence of item within the range of elements
        //     in the GenXdev.Containers.ThreadSafeList<T> that extends from the first element
        //     to index, if found; otherwise, –1.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is outside the range of valid indexes for the GenXdev.Containers.ThreadSafeList<T>.
        public int LastIndexOf(T item, int index)
        {
            lock (padLock)
            {
                return internalList.LastIndexOf(item, index);
            }
        }
        //
        // Summary:
        //     Searches for the specified object and returns the zero-based index of the
        //     last occurrence within the range of elements in the GenXdev.Containers.ThreadSafeList<T>
        //     that contains the specified number of elements and ends at the specified
        //     index.
        //
        // Parameters:
        //   item:
        //     The object to locate in the GenXdev.Containers.ThreadSafeList<T>. The value
        //     can be null for reference types.
        //
        //   index:
        //     The zero-based starting index of the backward search.
        //
        //   count:
        //     The number of elements in the section to search.
        //
        // Returns:
        //     The zero-based index of the last occurrence of item within the range of elements
        //     in the GenXdev.Containers.ThreadSafeList<T> that contains count number of elements
        //     and ends at index, if found; otherwise, –1.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is outside the range of valid indexes for the GenXdev.Containers.ThreadSafeList<T>.-or-count
        //     is less than 0.-or-index and count do not specify a valid section in the
        //     GenXdev.Containers.ThreadSafeList<T>.
        public int LastIndexOf(T item, int index, int count)
        {
            lock (padLock)
            {
                return internalList.LastIndexOf(item, index, count);
            }
        }
        //
        // Summary:
        //     Removes the first occurrence of a specific object from the GenXdev.Containers.ThreadSafeList<T>.
        //
        // Parameters:
        //   item:
        //     The object to remove from the GenXdev.Containers.ThreadSafeList<T>. The value
        //     can be null for reference types.
        //
        // Returns:
        //     true if item is successfully removed; otherwise, false. This method also
        //     returns false if item was not found in the GenXdev.Containers.ThreadSafeList<T>.
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        public bool Remove(T item)
        {
            lock (padLock)
            {
                return internalList.Remove(item);
            }
        }
        //
        // Summary:
        //     Removes all the elements that match the conditions defined by the specified
        //     predicate.
        //
        // Parameters:
        //   match:
        //     The System.Predicate<T> delegate that defines the conditions of the elements
        //     to remove.
        //
        // Returns:
        //     The number of elements removed from the GenXdev.Containers.ThreadSafeList<T>
        //     .
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     match is null.
        public int RemoveAll(Predicate<T> match)
        {
            lock (padLock)
            {
                return internalList.RemoveAll(match);
            }
        }
        //
        // Summary:
        //     Removes the element at the specified index of the GenXdev.Containers.ThreadSafeList<T>.
        //
        // Parameters:
        //   index:
        //     The zero-based index of the element to remove.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is less than 0.-or-index is equal to or greater than GenXdev.Containers.ThreadSafeList<T>.Count.
        public void RemoveAt(int index)
        {
            lock (padLock)
            {
                internalList.RemoveAt(index);
            }
        }
        //
        // Summary:
        //     Removes a range of elements from the GenXdev.Containers.ThreadSafeList<T>.
        //
        // Parameters:
        //   index:
        //     The zero-based starting index of the range of elements to remove.
        //
        //   count:
        //     The number of elements to remove.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is less than 0.-or-count is less than 0.
        //
        //   System.ArgumentException:
        //     index and count do not denote a valid range of elements in the GenXdev.Containers.ThreadSafeList<T>.
        public void RemoveRange(int index, int count)
        {
            lock (padLock)
            {
                internalList.RemoveRange(index, count);
            }
        }
        //
        // Summary:
        //     Reverses the order of the elements in the entire GenXdev.Containers.ThreadSafeList<T>.
        public void Reverse()
        {
            lock (padLock)
            {
                internalList.Reverse();
            }
        }
        //
        // Summary:
        //     Reverses the order of the elements in the specified range.
        //
        // Parameters:
        //   index:
        //     The zero-based starting index of the range to reverse.
        //
        //   count:
        //     The number of elements in the range to reverse.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is less than 0.-or-count is less than 0.
        //
        //   System.ArgumentException:
        //     index and count do not denote a valid range of elements in the GenXdev.Containers.ThreadSafeList<T>.
        public void Reverse(int index, int count)
        {
            lock (padLock)
            {
                internalList.Reverse(index, count);
            }
        }
        //
        // Summary:
        //     Sorts the elements in the entire GenXdev.Containers.ThreadSafeList<T> using
        //     the default comparer.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The default comparer System.Collections.Generic.Comparer<T>.Default cannot
        //     find an implementation of the System.IComparable<T> generic interface or
        //     the System.IComparable interface for type T.
        public void Sort()
        {
            lock (padLock)
            {
                internalList.Sort();
            }
        }
        //
        // Summary:
        //     Sorts the elements in the entire GenXdev.Containers.ThreadSafeList<T> using
        //     the specified System.Comparison<T>.
        //
        // Parameters:
        //   comparison:
        //     The System.Comparison<T> to use when comparing elements.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     comparison is null.
        //
        //   System.ArgumentException:
        //     The implementation of comparison caused an error during the sort. For example,
        //     comparison might not return 0 when comparing an item with itself.
        public void Sort(Comparison<T> comparison)
        {
            lock (padLock)
            {
                internalList.Sort(comparison);
            }
        }
        //
        // Summary:
        //     Sorts the elements in the entire GenXdev.Containers.ThreadSafeList<T> using
        //     the specified comparer.
        //
        // Parameters:
        //   comparer:
        //     The System.Collections.Generic.IComparer<T> implementation to use when comparing
        //     elements, or null to use the default comparer System.Collections.Generic.Comparer<T>.Default.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     comparer is null, and the default comparer System.Collections.Generic.Comparer<T>.Default
        //     cannot find implementation of the System.IComparable<T> generic interface
        //     or the System.IComparable interface for type T.
        //
        //   System.ArgumentException:
        //     The implementation of comparer caused an error during the sort. For example,
        //     comparer might not return 0 when comparing an item with itself.
        public void Sort(IComparer<T> comparer)
        {
            lock (padLock)
            {
                internalList.Sort(comparer);
            }
        }
        //
        // Summary:
        //     Sorts the elements in a range of elements in GenXdev.Containers.ThreadSafeList<T>
        //     using the specified comparer.
        //
        // Parameters:
        //   index:
        //     The zero-based starting index of the range to sort.
        //
        //   count:
        //     The length of the range to sort.
        //
        //   comparer:
        //     The System.Collections.Generic.IComparer<T> implementation to use when comparing
        //     elements, or null to use the default comparer System.Collections.Generic.Comparer<T>.Default.
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     index is less than 0.-or-count is less than 0.
        //
        //   System.ArgumentException:
        //     index and count do not specify a valid range in the GenXdev.Containers.ThreadSafeList<T>.-or-The
        //     implementation of comparer caused an error during the sort. For example,
        //     comparer might not return 0 when comparing an item with itself.
        //
        //   System.InvalidOperationException:
        //     comparer is null, and the default comparer System.Collections.Generic.Comparer<T>.Default
        //     cannot find implementation of the System.IComparable<T> generic interface
        //     or the System.IComparable interface for type T.
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            lock (padLock)
            {
                internalList.Sort(index, count, comparer);
            }
        }
        //
        // Summary:
        //     Copies the elements of the GenXdev.Containers.ThreadSafeList<T> to a new array.
        //
        // Returns:
        //     An array containing copies of the elements of the GenXdev.Containers.ThreadSafeList<T>.
        public T[] ToArray()
        {
            lock (padLock)
            {
                return internalList.ToArray();
            }
        }
        //
        // Summary:
        //     Sets the capacity to the actual number of elements in the GenXdev.Containers.ThreadSafeList<T>,
        //     if that number is less than a threshold value.
        public void TrimExcess()
        {
            lock (padLock)
            {
                internalList.TrimExcess();
            }
        }
        //
        // Summary:
        //     Determines whether every element in the GenXdev.Containers.ThreadSafeList<T>
        //     matches the conditions defined by the specified predicate.
        //
        // Parameters:
        //   match:
        //     The System.Predicate<T> delegate that defines the conditions to check against
        //     the elements.
        //
        // Returns:
        //     true if every element in the GenXdev.Containers.ThreadSafeList<T> matches the
        //     conditions defined by the specified predicate; otherwise, false. If the list
        //     has no elements, the return value is true.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     match is null.
        public bool TrueForAll(Predicate<T> match)
        {
            lock (padLock)
            {
                return internalList.TrueForAll(match);
            }
        }


        public bool IsReadOnly
        {
            get { return false; }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            lock (padLock)
            {
                return internalList.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (padLock)
            {
                return internalList.GetEnumerator();
            }
        }

        public void CopyTo(Array array, int index)
        {
            lock (padLock)
            {
                ((ICollection)internalList).CopyTo(array, index);
            }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return padLock; }
        }

        public int Add(object value)
        {
            lock (padLock)
            {
                return ((IList)internalList).Add(value);
            }
        }

        public bool Contains(object value)
        {
            lock (padLock)
            {
                return ((IList)internalList).Contains(value);
            }
        }

        public int IndexOf(object value)
        {
            lock (padLock)
            {
                return ((IList)internalList).IndexOf(value);
            }
        }

        public void Insert(int index, object value)
        {
            lock (padLock)
            {
                ((IList)internalList).Insert(index, value);
            }
        }

        public bool IsFixedSize
        {
            get
            {
                lock (padLock)
                {
                    return ((IList)internalList).IsFixedSize;
                }
            }
        }

        public void Remove(object value)
        {
            lock (padLock)
            {
                ((IList)internalList).Remove(value);
            }
        }

        object IList.this[int index]
        {
            get
            {
                lock (padLock)
                {
                    return ((IList)internalList)[index];
                }
            }
            set
            {
                lock (padLock)
                {
                    ((IList)internalList)[index] = value;
                }
            }
        }

        public bool TryDequeue(out T item)
        {
            item = default(T);

            lock (padLock)
            {
                if (internalList.Count == 0)
                {
                    return false;
                }

                item = internalList[0];
                internalList.RemoveAt(0);
            }

            return true;
        }

        public virtual void Update(ThreadSafeList<T> newItems, UpdateCallbackMatch matchCallback, UpdateCallbackUpdate updateCallback)
        {
            if (newItems == null)
                return;

            lock (padLock)
            {
                var newList = new List<T>(newItems.Count);

                foreach (var newItem in newItems)
                {
                    bool found = false;

                    foreach (var oldItem in internalList)
                    {
                        if (matchCallback(newItem, oldItem))
                        {
                            updateCallback(oldItem, newItem);

                            newList.Add(oldItem);

                            found = true;

                            break;
                        }
                    }

                    if (!found)
                    {
                        newList.Add(newItem);
                    }
                }

                internalList.Clear();
                internalList = newList;
            }
        }
    }
}

