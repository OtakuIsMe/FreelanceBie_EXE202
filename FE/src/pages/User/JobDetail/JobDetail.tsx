import React from "react";
import FPTLogo from "../../../assets/logo-fpt.jpg";
import Tag from "../../../components/Cards/Tag/Tag";
import "./JobDetail.css";
import Job from "../../../components/Cards/Job/Job";
import JobInfo from "../../../components/JobInfo/JobInfo";

const JobDetail: React.FC = () => {

	const sameJobs = Array(4).fill({
		company: "FPT Software",
		title: "Senior UX/UI Designer",
		location: "Ho Chi Minh",
		specialty: ["Web Design", "Mobile Design"],
		jType: "Full-time",
		timePosted: "1 hour ago",
	});

	const jobPosts = ({
		img: FPTLogo,
		company: "FPT Software",
		title: "Senior UX/UI Designer",
		location: "Ho Chi Minh",
		specialty: "Web Design",
		description: "<h3><strong>About this role</strong></h3><p>As an UI/UX Designer on Pixelz Studio, you'll focus on design user-friendly on several platform (web, mobile, dashboard, etc) to our users needs. Your innovative solution will enhance the user experience on several platforms. Join us and let's making impact on user engagement at Pixelz Studio.</p><p><br></p><h3><strong>Qualification</strong></h3><p>• At least 2-4 years of relevant experience in product design or related roles.</p><p>• Knowledge of design validation, either through quantitative or qualitative research.</p><p>• Have good knowledge using Figma and Figjam</p><p>• Experience with analytics tools to gather data from users.</p><p><br></p><h3><strong>Responsibility</strong></h3><p>• Create design and user journey on every features and product/business units across multiples devices (Web+App)</p><p>• Identifying design problems through user journey and devising elegant solutions</p><p>• Develop low and hi fidelity designs, user experience flow, &amp; prototype, translate it into highly-polished visual composites following style and brand guidelines.</p><p>• Brainstorm and works together with Design Lead, UX Engineers, and PMs to execute a design sprint on specific story or task</p>"
	});

	return (
		<div id="job_detail">
			<div className="job-detail-container">
				<div className="job-left-column">
					<JobInfo
						title={jobPosts.title}
						description={jobPosts.description}
						workType={0}
						workLocation={jobPosts.location}
						companyName={jobPosts.company}
						companyLogo={jobPosts.img}
						specialty={jobPosts.specialty}
						experience={2}
						employmentType={0}
						files={null}
					/>
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
								spectialty={job.specialty}
								jType={job.jType}
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
