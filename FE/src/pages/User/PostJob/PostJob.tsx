import React, { useEffect, useRef, useState } from 'react'
import './PostJob.css'
import { FaDollarSign } from 'react-icons/fa'
import Header from '../../../components/Header/Header';
import ReactQuill from 'react-quill';

interface postData {
	title: string;
	description: string;
	workType: number;
	workLocation: string;
	companyName: string;
	employmentType: number;
	experience: number;
	specialtyId: string;
	companyLogo: File | null;
	files: File[] | null;
	companyLink: string;
}

const PostJob: React.FC = () => {
	const [currentStep, setCurrentStep] = useState(1);
	const [data, setData] = useState<postData>(
		{
			title: '',
			description: '',
			workType: 0,
			workLocation: '',
			companyName: '',
			employmentType: 0,
			experience: 0,
			specialtyId: '',
			companyLogo: null,
			files: null,
			companyLink: ''
		}
	)

	useEffect(() => {
		console.log(data)
	}, [data])

	// const handleImageChange = (event: React.ChangeEvent<HTMLInputElement>) => {
	// 	const file = event.target.files?.[0];
	// 	if (file) {
	// 		const reader = new FileReader();
	// 		reader.onloadend = () => {
	// 			setImagePreview(reader.result as string);
	// 		};
	// 		reader.readAsDataURL(file);
	// 	}
	// };

	const handleNextStep = () => {
		setCurrentStep(prevStep => Math.min(prevStep + 1, 3));
	};

	const handlePreviousStep = () => {
		setCurrentStep(prevStep => Math.max(prevStep - 1, 1));
	};

	const handleOnChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		setData((prevData) => ({
			...prevData,
			[e.target.name]: e.target.value
		}));
	};

	const handleOnChangeDescription = (value: string) => {
		setData((prevData) => ({
			...prevData,
			description: value
		}));
	};

	const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const file = e.target.files?.[0];
		if (file) {
			setData((prev) => ({
				...prev,
				companyLogo: file,
			}));
		}
	};

	const handleOnChangeEmploymentType = (e: React.ChangeEvent<HTMLInputElement>) => {
		const employmentTypeMap: Record<string, number> = {
			fullTime: 0,
			partTime: 1,
			contract: 2
		};

		setData((prevData) => ({
			...prevData,
			employmentType: employmentTypeMap[e.target.value] ?? 0
		}));
	};

	const handleOnChangeWorkType = (e: React.ChangeEvent<HTMLInputElement>) => {
		const workTypeMap: Record<string, number> = {
			Remote: 0,
			Onsite: 1,
			Hybrid: 2
		};

		setData((prevData) => ({
			...prevData,
			workType: workTypeMap[e.target.value] ?? 0
		}));
	};

	return (
		<div id="post-job">
			<Header />
			<div className="popup-overlay-add-post">
				<div className="popup-content">
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
								<h3 className='step-label'>Step 1</h3>
								<h4 className='job-detail-label'>Job Details</h4>
								<form>
									<div className="form-group">
										<label htmlFor="title">Job Title</label>
										<input type="text" id="title" name="title" placeholder="e.g. Senior Product Designer"
											value={data.title}
											onChange={handleOnChange} />
									</div>
									<div className="form-group">
										<label>Add your job description</label>
										<ReactQuill value={data.description} onChange={handleOnChangeDescription} />
									</div>
									<div className="form-group">
										<label htmlFor="workplace">Workplace</label>
										<input type="text" id="workLocation" name="workLocation" placeholder='e.g. "New York City" or'
											value={data.workLocation}
											onChange={handleOnChange} />
									</div>
									<div className="company-information">
										<h4>Company Information</h4>
										<div className="form-group">
											<label htmlFor="companyName">What's your company name?</label>
											<input type="text" id="companyName" name="companyName" placeholder='e.g. "FreelanceBie" or ...'
												value={data.companyName}
												onChange={handleOnChange} />
										</div>
										<div className="form-group">
											<label htmlFor="companyLogo">Your company logo</label>
											<div className="file-input-wrapper">
												<label htmlFor="companyLogo" className="file-label">Choose image</label>
												<input type="file" id="companyLogo" name="companyLogo" onChange={handleFileChange} />
											</div>
											<small>Recommended dimensions: 144x144px</small>
											{data.companyLogo && <img src={URL.createObjectURL(data.companyLogo)} alt="Company Logo Preview" className="image-preview" />}
										</div>
										<div className="form-group">
											<label htmlFor="companyWebsite">Your company website</label>
											<input type="text" id="companyLink" name="companyLink" placeholder="e.g. https://domain.com"
												value={data.companyLink}
												onChange={handleOnChange} />
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
												<input
													type="radio"
													name="employmentType"
													value="fullTime"
													checked={data.employmentType === 0}
													onChange={handleOnChangeEmploymentType} />
												<span className="icon"><i className="fas fa-briefcase"></i></span> Full Time
											</label>
											<label className="employment-option">
												<input
													type="radio"
													name="employmentType"
													value="partTime"
													checked={data.employmentType === 1}
													onChange={handleOnChangeEmploymentType} />
												<span className="icon"><i className="fas fa-clipboard"></i></span> Part Time
											</label>
											<label className="employment-option">
												<input
													type="radio"
													name="employmentType"
													value="contract"
													checked={data.employmentType === 2}
													onChange={handleOnChangeEmploymentType} />
												<span className="icon"><i className="fas fa-hourglass"></i></span> Contract
											</label>
										</div>
									</div>

									<div className="form-group">
										<label>What type of design are you looking for?</label>
										<input type="text" placeholder="e.g. UI/UX Design" />
									</div>

									<div className="form-group">
										<label>Work type</label>
										<div className="work-type">
											<label className="work-option">
												<input type="radio" name="salaryType" value="Remote" />
												<span>Remote</span>
											</label>
											<label className="work-option">
												<input type="radio" name="salaryType" value="Onsite" />
												<span>Onsite</span>
											</label>
											<label className="work-option">
												<input type="radio" name="salaryType" value="Hybrid" />
												<span>Hybrid</span>
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
							</>
						)}
					</div>
					<div className="form-actions">
						<button type="button" className="cancel-btn" >Cancel</button>
						<div className='action-btns'>
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
		</div>
	)
}

export default PostJob;
