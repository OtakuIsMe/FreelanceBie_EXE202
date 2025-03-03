import React, { useRef, useState } from 'react'
import './PostJob.css'
import { FaDollarSign } from 'react-icons/fa'

const PostJob: React.FC = () => {
    const [currentStep, setCurrentStep] = useState(1);
    const quillRef = useRef<HTMLDivElement | null>(null);
    const [imagePreview, setImagePreview] = useState<string | null>(null);
    const [jobTitle] = useState('');
    const [jobDescription] = useState('');
    const [companyName] = useState('');
    const [companyLogo] = useState('');

    const handleImageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (file) {
            const reader = new FileReader();
            reader.onloadend = () => {
                setImagePreview(reader.result as string);
            };
            reader.readAsDataURL(file);
        }
    };

    const handleNextStep = () => {
        setCurrentStep(prevStep => Math.min(prevStep + 1, 3));
    };

    const handlePreviousStep = () => {
        setCurrentStep(prevStep => Math.max(prevStep - 1, 1));
    };

    return (
        <div id="post-job">
            <div className="popup-overlay-add-post">
                <div className="popup-content">
                    <button className="close-btn-add-post">×</button>
                    <h2 className="popup-title">Post a Design Job</h2>
                    <p className="popup-description">Create job board for hiring designers</p>
                    <div className="step-indicator">
                        <div className={`step ${currentStep === 1 ? 'active' : ''}`}>
                            <span className="step-number">1</span>
                            <span className="step-label">Job Details</span>
                        </div>
                        <div className={`step ${currentStep === 2 ? 'active' : ''}`}>
                            <span className="step-number">2</span>
                            <span className="step-label">Requirement & Date</span>
                        </div>
                        <div className={`step ${currentStep === 3 ? 'active' : ''}`}>
                            <span className="step-number">3</span>
                            <span className="step-label">Confirm & Complete</span>
                        </div>
                    </div>
                    <div className="form-step">
                        {currentStep === 1 && (
                            <>
                                <h3>Step 1</h3>
                                <h4>Job Details</h4>
                                <form>
                                    <div className="form-group">
                                        <label htmlFor="title">Job Title</label>
                                        <input type="text" id="title" name="title" placeholder="e.g. Senior Product Designer" />
                                    </div>
                                    <div className="form-group">
                                        <label>Add your job description</label>
                                        <div className="rich-text-editor">
                                            <div ref={quillRef} />
                                        </div>
                                    </div>
                                    <div className="form-group">
                                        <label htmlFor="workplace">Workplace Type</label>
                                        <input type="text" id="workplace" name="workplace" placeholder='e.g. "New York City" or' />
                                    </div>
                                    <div className="company-information">
                                        <h4>Company Information</h4>
                                        <div className="form-group">
                                            <label htmlFor="companyName">What's your company name?</label>
                                            <input type="text" id="companyName" name="companyName" placeholder='e.g. "FreelanceBie" or ...' />
                                        </div>
                                        <div className="form-group">
                                            <label htmlFor="companyLogo">Your company logo</label>
                                            <div className="file-input-wrapper">
                                                <label htmlFor="companyLogo" className="file-label">Choose image</label>
                                                <input type="file" id="companyLogo" name="companyLogo" onChange={handleImageChange} />
                                            </div>
                                            <small>Recommended dimensions: 144x144px</small>
                                            {imagePreview && <img src={imagePreview} alt="Company Logo Preview" className="image-preview" />}
                                        </div>
                                        <div className="form-group">
                                            <label htmlFor="companyWebsite">Your company website</label>
                                            <input type="text" id="companyWebsite" name="companyWebsite" placeholder="e.g. https://domain.com" />
                                        </div>
                                    </div>

                                </form>
                            </>
                        )}
                        {currentStep === 2 && (
                            <div className="step-2">
                                <h3>Step 2</h3>
                                <h4>Requirement & Date</h4>
                                <form>
                                    <div className="form-group">
                                        <label>Employment type</label>
                                        <div className="employment-type">
                                            <label className="employment-option">
                                                <input type="radio" name="employmentType" value="fullTime" />
                                                <span className="icon"><i className="fas fa-briefcase"></i></span> Full Time
                                            </label>
                                            <label className="employment-option">
                                                <input type="radio" name="employmentType" value="partTime" />
                                                <span className="icon"><i className="fas fa-clipboard"></i></span> Part Time
                                            </label>
                                            <label className="employment-option">
                                                <input type="radio" name="employmentType" value="contract" />
                                                <span className="icon"><i className="fas fa-hourglass"></i></span> Contract
                                            </label>
                                        </div>
                                    </div>

                                    <div className="form-group">
                                        <label>What type of design are you looking for?</label>
                                        <input type="text" placeholder="e.g. UI/UX Design" />
                                    </div>

                                    <div className="form-group">
                                        <label>Salary type</label>
                                        <div className="salary-type">
                                            <label className="salary-option">
                                                <input type="radio" name="salaryType" value="hourly" />
                                                <span>Hourly</span>
                                            </label>
                                            <label className="salary-option">
                                                <input type="radio" name="salaryType" value="monthly" />
                                                <span>Monthly</span>
                                            </label>
                                        </div>
                                    </div>

                                    <div className="form-group">
                                        <label>Payment rate</label>
                                        <div className="payment-rate">
                                            <span>
                                                <FaDollarSign />
                                            </span>
                                            <input type="number" placeholder="10" />
                                            <span>/hour</span>
                                        </div>
                                    </div>
                                </form>
                            </div>
                        )}
                        {currentStep === 3 && (
                            <>
                                <h3>Step 3</h3>
                                <h4>Confirm & Complete</h4>
                                {/* Display job title and job description from Step 1 */}
                                <div>
                                    <h5>Job Title:</h5>
                                    <p>{jobTitle}</p>
                                    <h5>Job Description:</h5>
                                    <p>{jobDescription}</p>
                                </div>
                                {/* Display company name and company logo from Step 2 */}
                                <div>
                                    <h5>Company Name:</h5>
                                    <p>{companyName}</p>
                                    <h5>Company Logo:</h5>
                                    <img src={companyLogo} alt="Company Logo" />
                                </div>
                            </>
                        )}
                    </div>
                    <div className="form-actions">
                        <button type="button" className="cancel-btn" >Cancel</button>
                        {currentStep > 1 && (
                            <button type="button" className="back-btn" onClick={handlePreviousStep}>Back</button>
                        )}
                        {currentStep < 3 ? (
                            <button type="button" className="continue-btn" onClick={handleNextStep}>Continue</button>
                        ) : (
                            <button type="submit" className="submit-btn">Submit</button>
                        )}
                    </div>
                </div>
            </div>
        </div>
    )
}

export default PostJob;