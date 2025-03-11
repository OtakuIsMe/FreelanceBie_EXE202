import React from 'react'
import './Job.css'

interface Job {
	img: string;
	company: string;
	title: string;
	location: string;
	timePosted: number;
	mini: boolean;
}

const Job: React.FC<Job> = ({ img, company, title, location, timePosted, mini }) => {

	function timeAgo(seconds: number): string {
		const minute = 60;
		const hour = 60 * minute;
		const day = 24 * hour;

		if (seconds >= day) {
			const days = Math.floor(seconds / day);
			return `${days} day${days > 1 ? 's' : ''} ago`;
		} else if (seconds >= hour) {
			const hours = Math.floor(seconds / hour);
			return `${hours} hour${hours > 1 ? 's' : ''} ago`;
		} else if (seconds >= minute) {
			const minutes = Math.floor(seconds / minute);
			return `${minutes} minute${minutes > 1 ? 's' : ''} ago`;
		} else {
			return "Just now";
		}
	}

	return mini ? (
		<div id="job_card">
			<div className="job-info mini">
				<img
					src={img}
					alt="Company Logo"
					className="company-logo"
				/>
				<div>
					<h5>{title}</h5>
					<p><b>{company}</b> - {location}</p>
				</div>
			</div>
			<div className="job-meta mini">
				<p>{timePosted}</p>
			</div>
		</div>
	) : (
		<div id="job_card">
			<div className="job-info">
				<img
					src={img}
					alt="Company Logo"
					className="company-logo"
				/>
				<div>
					<h4>{company}</h4>
					<p>{title}</p>
				</div>
			</div>
			<div className="job-meta">
				<p>{timeAgo(timePosted)}</p>
				<p>{location}</p>
			</div>
		</div>
	);
}

export default Job
