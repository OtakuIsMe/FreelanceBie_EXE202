import React, { useEffect, useState } from "react";
import FPTLogo from "../../../assets/logo-fpt.jpg";
import "./JobDetail.css";
import Job from "../../../components/Cards/Job/Job";
import JobInfo, { FileProps } from "../../../components/JobInfo/JobInfo";
import { useParams } from "react-router-dom";
import { ApiGateway } from "../../../services/api/ApiService";

import { ReactNotifications } from 'react-notifications-component'
import 'react-notifications-component/dist/theme.css'
import { Store } from 'react-notifications-component';

interface JobPostProps {
	title: string;
	description: string;
	workType: number;
	workLocation: string;
	companyName: string;
	companyLogo: string;
	specialty: string;
	experience: number;
	employmentType: number;
	files: FileProps[];
}
interface statusObj {
	status: boolean
}
const JobDetail: React.FC = () => {
	const { id } = useParams();
	const [jobPosts, setJobPost] = useState<JobPostProps | null>(null)
	const [isApply, setIsApply] = useState<boolean>(false)

	const sameJobs = Array(4).fill({
		company: "FPT Software",
		title: "Senior UX/UI Designer",
		location: "Ho Chi Minh",
		specialty: ["Web Design", "Mobile Design"],
		jType: "Full-time",
		timePosted: "1 hour ago",
	});
	useEffect(() => {
		fetchingJobDetail();
		CheckApply();
	}, [id])

	const CheckApply = async () => {
		if (id) {
			const data = await ApiGateway.CheckApply<statusObj>(id)
			setIsApply(data.status)
		}
	}

	const fetchingJobDetail = async () => {
		console.log(id)
		if (!id) {
			console.error("ID is undefined, cannot fetch job details.");
			return;
		}

		const data = await ApiGateway.JobDetail<JobPostProps>(id);
		setJobPost(data);
	};

	const handleApplySuccess = () => {
		Store.addNotification({
			title: "Apply Success",
			message: "Your Profile will be sending to employer",
			type: "success",
			insert: "top",
			container: "top-right",
			animationIn: ["animate__animated", "animate__fadeIn"],
			animationOut: ["animate__animated", "animate__fadeOut"],
			dismiss: {
				duration: 5000,
				onScreen: true
			}
		});
		setIsApply(true)
	}

	return (
		<div id="job_detail">
			<ReactNotifications />
			<div className="job-detail-container">
				<div className="job-left-column">
					{jobPosts && (
						<JobInfo
							id={id ?? ""}
							title={jobPosts.title}
							description={jobPosts.description}
							workType={jobPosts.workType}
							workLocation={jobPosts.workLocation}
							companyName={jobPosts.companyName}
							companyLogo={jobPosts.companyLogo}
							specialty={jobPosts.specialty}
							experience={jobPosts.experience}
							employmentType={jobPosts.employmentType}
							files={jobPosts.files}
							applyNoti={handleApplySuccess}
							isApply={isApply}
						/>
					)}
				</div>

				<div className="job-right-column">
					<div className="similar-jobs">
						<p className="tittle">Similar jobs</p>
						{sameJobs.map((job, index) => (
							<Job
								key={index}
								img={FPTLogo}
								company={job.company}
								title={job.title}
								location={job.location}
								timePosted={job.timePosted}
								mini={true}
							/>
						))}
					</div>
				</div>
			</div>
		</div>
	);
};

export default JobDetail;
