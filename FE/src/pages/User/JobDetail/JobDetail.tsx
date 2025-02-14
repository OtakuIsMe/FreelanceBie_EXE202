import React from "react";
import FPTLogo from "../../../assets/logo-fpt.jpg";
import Tag from "../../../components/Cards/Tag/Tag";
import "./JobDetail.css";

const JobDetail: React.FC = () => {

    const jobPosts = ({
        img: FPTLogo,
        company: "FPT Software",
        title: "Senior UX/UI Designer",
        location: "Ho Chi Minh",
        specialty: ["Web Design", "Mobile Design"],
        jType: "Full-time",
        timePosted: "1 hour ago",
        qualification: `At least 2-4 years of relevant experience in product design or related roles.
Knowledge of design validation, either through quantitative or qualitative research.
Good knowledge using Figma and Figjam.
Experience with analytics tools to gather data from users.`,
        responsibility: `Create design and user journey on every feature and product across multiple devices.
Identify design problems and propose solutions.
Develop low and high-fidelity designs, UX flow, and prototypes.
Brainstorm with team members to execute design sprints.`
    });

    return (
        <div id="job_detail">
            <div className="job-detail-container">
                <div className="job-left-column">
                    <div className="job-header">
                        <div className="top">
                            <h1>{jobPosts.title}</h1>
                            <button className="apply-button">Apply Now</button>
                        </div>
                        <div className="bottom">
                            <div className="img-container">
                               <img src={jobPosts.img} alt={jobPosts.company} />
                            </div>
                            <div className="comp-info">
                                <p className="company">{jobPosts.company} - 📍 {jobPosts.location}</p>
                                <div className="job-meta">
                                    <Tag tag={jobPosts.jType}/>
                                    <Tag tag="Remote"/>
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <div className="job-description">
                        <p className="tittle">About this role</p>
                        <p>
                            As an UI/UX Designer on Pixelz Studio, you'll focus on design user-friendly on several platform (web, mobile, dashboard, etc) to our users needs. Your innovative solution will enhance the user experience on several platforms. Join us and let's making impact on user engagement at Pixelz Studio.
                        </p>
                    </div>
                    
                    <div className="job-qualifications">
                        <p className="tittle">Qualification</p>
                        <pre>
                            {jobPosts.qualification}
                        </pre>
                    </div>
                    
                    <div className="job-responsibilities">
                        <p className="tittle">Responsibility</p>
                        <pre>
                            {jobPosts.responsibility}
                        </pre>
                    </div>
                    
                    <div className="attachments">
                        <p className="tittle">Attachments</p>
                        <div className="attachment-files">
                            <span>📄 Jobs_Requirements.pdf</span>
                            <span>📄 Company_Benefits.pdf</span>
                        </div>
                    </div>
                </div>

                <div className="job-right-column">
                    <div className="similar-jobs">
                        <p className="tittle">Similar jobs</p>
                        <ul>
                            <li>Lead UI Designer - Cogitek - Ha Noi, Viet Nam</li>
                            <li>Lead UI Designer - Cogitek - Ha Noi, Viet Nam</li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default JobDetail;
