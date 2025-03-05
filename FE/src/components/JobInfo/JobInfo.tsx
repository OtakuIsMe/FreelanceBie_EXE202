import React from 'react'
import './JobInfo.css'
import Tag from '../Cards/Tag/Tag';

interface JobInfoProps {
	title: string;
	description: string;
	workType: number;
	workLocation: string;
	companyName: string;
	companyLogo: File | string | null;
	specialty: string;
	experience: number;
	employmentType: number;
	files: File[] | null;
}

const JobInfo: React.FC<JobInfoProps> = (props) => {

	const employmentTypeLabels = ["Full Time", "Part Time", "Contract"];
	const workTypeLabels = ["Remote", "Onsite", "Hybrid"];

	return (
		<div id="job-info">
			<div className="job-header">
				<div className="top">
					<h1>{props.title}</h1>
					<button className="apply-button">Apply Now</button>
				</div>
				<div className="bottom">
					<div className="img-container">
						{props.companyLogo && (
							<img
								src={typeof props.companyLogo === "string" ? props.companyLogo : URL.createObjectURL(props.companyLogo)}
								alt={props.companyName}
							/>
						)}
					</div>
					<div className="comp-info">
						<p className="company">{props.companyName} - 📍 {props.workLocation}</p>
						<div className="job-meta">
							<Tag tag={props.specialty} />
							<Tag tag={employmentTypeLabels[props.employmentType]} />
							<Tag tag={workTypeLabels[props.workType]} />
							<Tag tag={`${props.experience} years`} />
						</div>
					</div>
				</div>
			</div>

			<div className="job-description" dangerouslySetInnerHTML={{ __html: props.description || "" }}></div>

			<div className="attachments">
				<p className="tittle">Attachments</p>
				<div className="attachment-files">
					<span>📄 Jobs_Requirements.pdf</span>
					<span>📄 Company_Benefits.pdf</span>
				</div>
			</div>
		</div>
	)
}

export default JobInfo
