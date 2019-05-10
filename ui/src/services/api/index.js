/**
 * An error that is thrown if an API call produced an error.
 */
export class ApiError extends Error {
	constructor(message, details, code) {
		super();
		this.message = message;
		this.details = details;
		this.code = code;
	}
}

/**
 * Handle the error response from the API server and throw an appropriate
 * exception.
 *
 * @param error The error to handle.
 */
export function handleError(error) {
	if (error.response) {
		const msg = (error.response.data
			&& error.response.data.message) || 'Unknown Error';
		let details = (error.response.data && error.response.data.details) || [];
		details = details.reduce((map, detail) => {
			map[detail.field] = detail.message;
			return map;
		}, {});
		const code = (error.response.data && error.response.data.code) || undefined;
		throw new ApiError(msg, details, code);
	}

	throw error;
}
