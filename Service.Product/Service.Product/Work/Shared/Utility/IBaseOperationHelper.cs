namespace Service.Product.Shared.Utility
{
    public interface IBaseOperationHelper<TEntity>
    {
        /// <summary>
        /// Perform base sorting operation.
        /// </summary>
        /// <remarks>
        /// Valid order:
        ///     [name, product]
        ///     [-name]
        ///     [-name, product]
        ///     [-name, -product]
        /// </remarks>
        /// <param name="entity">IQueryable&lt;TEntity&gt;</param>
        /// <param name="parameter">Sorting object parameter</param>
        /// <param name="parameterKey">Object key to search for</param>
        /// <returns>Sorted IQueryable&lt;TEntity&gt;</returns>
        IQueryable<TEntity> Sort(IQueryable<TEntity> entity, object parameter, string parameterKey);

        /// <summary>
        /// Perform base single filtering operation.
        /// </summary>
        /// <remarks>
        ///     Filter based on Id by default.
        /// </remarks>
        /// <param name="entity">IQueryable&lt;TEntity&gt;</param>
        /// <param name="parameter">Filtering single object parameter</param>
        /// <param name="parameterKey">Object key to search for</param>
        /// <returns>Filtered IQueryable&lt;TEntity&gt;</returns>
        IQueryable<TEntity> FilterSingle(IQueryable<TEntity> entity, object parameter, string parameterKey);

        /// <summary>
        /// Perform base range filtering operation.
        /// </summary>
        /// <param name="entity">IQueryable&lt;TEntity&gt;</param>
        /// <param name="parameter">Filtering range object parameter</param>
        /// <returns>Filtered IQueryable&lt;TEntity&gt;</returns>
        IQueryable<TEntity> FilterRange(IQueryable<TEntity> entity, object parameter, string parameterKey);

        /// <summary>
        /// Perform all base sorting and filtering operation.
        /// <para>
        /// OrderBy, FilterBy, RangeFilterBy
        /// </para>
        /// </summary>
        /// <param name="entity">IQueryable&lt;TEntity&gt;</param>
        /// <param name="parameter">Filtering range object parameter</param>
        /// <returns>Sorted and filtered IQueryable&lt;TEntity&gt;</returns>
        IQueryable<TEntity> RunAll(IQueryable<TEntity> entity, object parameter);

    }
}