import { useState } from 'react';
import Header from '../../../../components/Header/Header';
import './PostManage.css'

interface jobCard {
    title: string,
    status: boolean,
    workType: number,
    workLocation: string,
    employmentType: number,
    createAt: string,
    closeAt: string,
    numberApplied: number,
    numberHired: number,
    id: string
}

const PostManage = () => {
    const [jobs, setJobs] = useState<jobCard[]>([])
    return (
        <div id="post-manage">
            <Header />
            <div className="manage-container">
                <div className="title-container">
                    <p className="title">Post Opening</p>
                </div>
                <div className="status-counting">
                    <div className="status">
                        <p>Open</p>
                    </div>
                    <div className="status">
                        <p>Close</p>
                    </div>
                </div>
                <div className="action-bar">
                    <div className="search-bar"></div>
                    <div className="categories selection-bar"></div>
                    <div className="statuses selection-bar"></div>
                </div>
                <div className="jobs-container">
                    {jobs.map((job, index) => {
                        return (
                            <div className="job-card" key={index}>
                                <div className="status-edit-btn">
                                </div>
                                <div className="title-job-container">
                                    <p className="title"></p>
                                </div>
                                <div className="tags">
                                    <div className="tag">
                                        <p></p>
                                    </div>
                                    <div className="tag">
                                        <p></p>
                                    </div>
                                    <div className="tag">
                                        <p></p>
                                    </div>
                                </div>
                                <div className="counting-container">
                                    <div className="applied"></div>
                                    <div className="hiring"></div>
                                </div>
                                <div className="plan-container">
                                    <div className="time-container">
                                        <div className="duration-time">
                                            <span></span>
                                        </div>
                                        <div className="time-left">
                                            <span></span>
                                        </div>
                                    </div>
                                    <div className="progress-bar">

                                    </div>
                                </div>
                                <div className="detail-container">
                                    <p className="detail">View Details</p>
                                </div>
                            </div>
                        )
                    })}
                </div>
            </div>
        </div>
    )
}

export default PostManage;