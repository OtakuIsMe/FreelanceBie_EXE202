import React, { useEffect, useRef, useState } from 'react'
import './PostJob.css'
import { FaDollarSign } from 'react-icons/fa'
import Header from '../../../components/Header/Header';
import ReactQuill from 'react-quill';
import JobInfo from '../../../components/JobInfo/JobInfo';
import PDF from '../../../assets/pdf.png'
import DOCX from '../../../assets/docx.png'
import CheckOut from '../../../components/CheckOut/CheckOut';
import { ApiGateway } from '../../../services/api/ApiService';
import Utils from '../../../helper/Utils';

export interface postData {
	title: string;
	description: string;
	workType: number;
	workLocation: string;
	companyName: string;
	employmentType: number;
	experience: number;
	specialty: string;
	companyLogo: File | null;
	files: File[] | null;
	companyLink: string;
	payment: number;
}

interface statusObj {
	status: boolean
}

const PostJob: React.FC = () => {
	const [currentStep, setCurrentStep] = useState(1);
	const fileInputRef = useRef<HTMLInputElement | null>(null);
	const [isOpenPopup, setIsOpenPopup] = useState<boolean>(false)
	const [isBuyMembership, setIsBuyMembership] = useState<boolean>(false)
	const [data, setData] = useState<postData>(
		{
			title: '',
			description: '',
			workType: 0,
			workLocation: '',
			companyName: '',
			employmentType: 0,
			experience: 0,
			specialty: '',
			companyLogo: null,
			files: null,
			companyLink: '',
			payment: 0
		}
	)

	useEffect(() => {
		fetchCheckMembership()
	}, [])

	const fetchCheckMembership = async () => {
		const data = await ApiGateway.CheckMembership<statusObj>();
		setIsBuyMembership(data.status)
	}
	const handleFilesChange = (event: React.ChangeEvent<HTMLInputElement>) => {
		const files = event.target.files;
		if (files) {
			const validFiles = Array.from(files).filter(
				(file) =>
					file.type === "application/pdf" ||
					file.type === "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
			);

			setData((prevData) => ({
				...prevData,
				files: validFiles,
			}));

			if (validFiles.length === 0) {
				alert("Only PDF and DOCX files are allowed.");
			}
		}
	};

	const handleButtonClick = () => {
		if (fileInputRef.current) {
			fileInputRef.current.click();
		}
	};

	const handleRemoveFile = (index: number) => {
		setData((prevData) => ({
			...prevData,
			files: prevData.files ? prevData.files.filter((_, i) => i !== index) : null,
		}));
	};

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

	const handleNumberChange = (e: React.ChangeEvent<HTMLInputElement>) => {
		const { name, value } = e.target;

		setData((prevData) => ({
			...prevData,
			[name]: value === '' ? null : Number(value)
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

	const UploadJob = async () => {
		if (isBuyMembership) {
			const response = await ApiGateway.AddPostJob(data);
			if (response) {
				window.location.href = '/post/manage'
			}
		} else {
			const { companyLogo, files, ...postDataWithoutFiles } = data;

			// Lưu dữ liệu text vào localStorage
			localStorage.setItem("pendingPostJob", JSON.stringify(postDataWithoutFiles));

			// Lưu File vào sessionStorage dưới dạng Base64
			if (companyLogo) {
				const reader = new FileReader();
				reader.onload = () => {
					sessionStorage.setItem("pendingPostJob_companyLogo", reader.result as string);
				};
				reader.readAsDataURL(companyLogo);
			}

			if (files) {
				files.forEach((file, index) => {
					const reader = new FileReader();
					reader.onload = () => {
						sessionStorage.setItem(`pendingPostJob_files_${index}`, reader.result as string);
					};
					reader.readAsDataURL(file);
				});
			}

			// Gọi API tạo payment URL
			const url = await ApiGateway.PaymentUrl(
				Utils.generateRandomCode(),
				Utils.convertUsdToVnd(1),
				"Add post",
				"post"
			);

			// Chuyển hướng đến trang thanh toán
			window.location.href = url;
		}
	};

	return (
		<div id="post-job">
			<Header />
			{isOpenPopup && (
				<CheckOut amount={1}
					closePopup={() =>
						setIsOpenPopup(false)}
					ActionAfterPayment={UploadJob} />
			)}
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
							<div className="step-1">
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
							</div>
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
										<input type="text" name="specialty" placeholder="e.g. UI/UX Design"
											value={data.specialty}
											onChange={handleOnChange} />
									</div>

									<div className="form-group">
										<label>Work type</label>
										<div className="work-type">
											<label className="work-option">
												<input type="radio" name="salaryType"
													value="Remote"
													checked={data.workType === 0}
													onChange={handleOnChangeWorkType} />
												<span>Remote</span>
											</label>
											<label className="work-option">
												<input type="radio" name="salaryType"
													value="Onsite"
													checked={data.workType === 1}
													onChange={handleOnChangeWorkType} />
												<span>Onsite</span>
											</label>
											<label className="work-option">
												<input type="radio" name="salaryType"
													value="Hybrid"
													checked={data.workType === 2}
													onChange={handleOnChangeWorkType} />
												<span>Hybrid</span>
											</label>
										</div>
									</div>

									<div className="form-group">
										<label>Experience require</label>
										<input type="number" placeholder="2"
											name="experience"
											value={data.experience === 0 ? '' : data.experience}
											onChange={handleNumberChange} />
									</div>

									<div className="form-group">
										<label>Payment rate</label>
										<div className="payment-rate">
											<span>
												<FaDollarSign />
											</span>
											<input type="number" placeholder="10"
												name="payment"
												value={data.payment === 0 ? '' : data.payment}
												onChange={handleNumberChange} />
											<span>/hour</span>
										</div>
									</div>
									<div className="form-group">
										<label>Attachments</label>
										<div className="input-attachments">
											<input type="file" name="" id=""
												ref={fileInputRef} style={{ display: 'none' }}
												onChange={handleFilesChange}
												multiple
												accept=".pdf, .docx" />
											<div className='btn-file' onClick={handleButtonClick}>
												<svg
													aria-hidden="true"
													stroke="currentColor"
													stroke-width="2"
													viewBox="0 0 24 24"
													fill="none"
													xmlns="http://www.w3.org/2000/svg"
												>
													<path
														stroke-width="2"
														stroke="#fffffff"
														d="M13.5 3H12H8C6.34315 3 5 4.34315 5 6V18C5 19.6569 6.34315 21 8 21H11M13.5 3L19 8.625M13.5 3V7.625C13.5 8.17728 13.9477 8.625 14.5 8.625H19M19 8.625V11.8125"
														stroke-linejoin="round"
														stroke-linecap="round"
													></path>
													<path
														stroke-linejoin="round"
														stroke-linecap="round"
														stroke-width="2"
														stroke="#fffffff"
														d="M17 15V18M17 21V18M17 18H14M17 18H20"
													></path>
												</svg>
												ADD FILE
											</div>
											{data.files && data.files.length > 0 && (
												<div className="file-list">
													{data.files.map((file, index) => (
														<div key={index} className="file-preview">
															<div className='img-info'>
																<img
																	src={file.type === "application/pdf" ? PDF : DOCX}
																	alt="File Type"
																	className="file-icon"
																/>
																<div className="file-info">
																	<a href={URL.createObjectURL(file)} target="_blank" rel="noopener noreferrer">
																		{file.name}
																	</a>
																	<span>{(file.size / 1024).toFixed(2)} KB</span>
																</div>
															</div>
															<div className="delete-btn" onClick={() => handleRemoveFile(index)}>
																<svg xmlns="http://www.w3.org/2000/svg" width="28" height="28" viewBox="0 0 28 28"><path fill="currentColor" d="M11.5 6h5a2.5 2.5 0 0 0-5 0M10 6a4 4 0 0 1 8 0h6.25a.75.75 0 0 1 0 1.5h-1.31l-1.217 14.603A4.25 4.25 0 0 1 17.488 26h-6.976a4.25 4.25 0 0 1-4.235-3.897L5.06 7.5H3.75a.75.75 0 0 1 0-1.5zM7.772 21.978a2.75 2.75 0 0 0 2.74 2.522h6.976a2.75 2.75 0 0 0 2.74-2.522L21.436 7.5H6.565zM11.75 11a.75.75 0 0 1 .75.75v8.5a.75.75 0 0 1-1.5 0v-8.5a.75.75 0 0 1 .75-.75m5.25.75a.75.75 0 0 0-1.5 0v8.5a.75.75 0 0 0 1.5 0z" /></svg>
															</div>
														</div>
													))}
												</div>
											)}
										</div>
									</div>
								</form>
							</div>
						)}
						{currentStep === 3 && (
							<>
								<h3>Step 3</h3>
								<h4>Confirm & Complete</h4>
								<JobInfo
									id=''
									title={data.title}
									description={data.description}
									workType={data.workType}
									workLocation={data.workLocation}
									companyName={data.companyName}
									companyLogo={data.companyLogo}
									specialty={data.specialty}
									experience={data.experience}
									employmentType={data.employmentType}
									files={data.files}
									applyNoti={() => { }}
									isApply={false}
								/>
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
								<button type="submit" className="submit-btn" onClick={UploadJob}>Submit</button>
							)}
						</div>
					</div>
				</div>
			</div>
		</div>
	)
}

export default PostJob;
