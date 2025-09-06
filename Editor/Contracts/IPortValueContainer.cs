#nullable enable

namespace Misaki.GraphProcessor.Editor
{
    /// <summary>
    /// Interface for a container that holds ports and allows access to their values.
    /// </summary>
    public interface IPortValueContainer
    {
        /// <summary>
        /// Gets the value of an input port by its name.
        /// </summary>
        /// <typeparam name="T">The expected type of the port value.</typeparam>
        /// <param name="portName">The name of the input port.</param>
        /// <returns>The value of the input port, cast to the specified type.</returns>
        public T? GetInputPortValue<T>(string portName);

        /// <summary>
        /// Gets the value of a port by its name.
        /// </summary>
        /// <param name="portName">The name of the port.</param>
        /// <returns>The value of the port, or null if the port does not exist or has no value.</returns>
        public object? GetPortValue(string portName);

        /// <summary>
        /// Sets the value of a port by its name.
        /// </summary>
        /// <param name="portName">The name of the port.</param>
        /// <param name="value">The value to set for the port.</param>
        public void SetPortValue(string portName, object? value);
    }
}