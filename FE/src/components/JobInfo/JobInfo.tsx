import React from 'react'
import './JobInfo.css'
import Tag from '../Cards/Tag/Tag';

import PDF from '../../assets/pdf.png'
import DOCX from '../../assets/docx.png'

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
	files: File[] | FileProps[] | null;
}

export interface FileProps {
	id: string;
	name: string,
	type: string,
	size: number,
	url: string
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
			{props.files && props.files.length > 0 && (
				<div className="attachments">
					<p className="title">Attachments</p>
					<div className="attachment-files">
						<div className="file-list">
							{props.files.map((file, index) => (
								<div key={index} className="file-preview">
									<div className='img-info'>
										<img
											src={file.type.toLowerCase().includes("pdf") ? PDF : DOCX}
											alt="File Type"
											className="file-icon"
										/>
										<div className="file-info">
											<a href={file instanceof File ? URL.createObjectURL(file) : file.url} target="_blank" rel="noopener noreferrer">
												{file.name}
											</a>
											<span>{(file.size / 1024).toFixed(2)} KB</span>
										</div>
									</div>
								</div>
							))}
						</div>
					</div>
				</div>
			)}
		</div>
	)
}

export default JobInfo
