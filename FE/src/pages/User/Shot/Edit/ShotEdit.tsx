import './ShotEdit.css'
import image from '../../../../assets/image.png'
import { useRef, useState } from 'react';
import ReactQuill from 'react-quill';
import 'react-quill/dist/quill.snow.css';
import { RxText } from "react-icons/rx";
import { RxImage } from "react-icons/rx";
import { RxVideo } from "react-icons/rx";

interface contentBlock {
	type: string,
	data: string,
	AltText: string,
	isMinimize: boolean
}
const ShotEdit = () => {
	const fileInputRef = useRef<HTMLInputElement | null>(null);
	const fileInputSideRef = useRef<HTMLInputElement | null>(null);
	const [titleShot, setTitle] = useState<string>("");
	const [contentBlocks, setContentBlocks] = useState<contentBlock[] | null>(null);
	const [sideAction, setSideAction] = useState<string>("")
	const [focusAction, setFocusAction] = useState<number | null>(null)
	const [textFocus, setTextFocus] = useState<number>(-1)
	const handleMediaClick = () => {
		if (fileInputRef.current) {
			fileInputRef.current.click();
		}
	};
	const handleChangeClick = () => {
		if (fileInputSideRef.current) {
			fileInputSideRef.current.click();
		}
	}
	const modules = {
		toolbar: {
			container: ".custome-edior-tool",
		},
	};

	const handleAddFile = (event: React.ChangeEvent<HTMLInputElement>) => {
		if (event.target.files && event.target.files.length > 0) {
			const url = URL.createObjectURL(event.target.files[0]);
			setContentBlocks([{
				type: 'image',
				data: url,
				AltText: '',
				isMinimize: false
			}])
			setSideAction("insert");
		}
	};

	const handleChangeImage = (event: React.ChangeEvent<HTMLInputElement>) => {
		if (event.target.files && event.target.files.length > 0 && focusAction != null) {
			const url = URL.createObjectURL(event.target.files[0]);
			console.log(url)
			handleDataChange(url, focusAction)
		}
	}

	const hanldeTitleChange = (event: React.ChangeEvent<HTMLTextAreaElement>) => {
		setTitle(event.target.value);
	}

	const handleDataChange = (value: string, index: number) => {
		setContentBlocks((prevBlocks) => {
			if (!prevBlocks) return null;

			const updatedBlocks = [...prevBlocks];
			updatedBlocks[index] = { ...updatedBlocks[index], data: value };

			return updatedBlocks;
		});
	}

	const handleSideActionChange = (value: string) => {
		setSideAction(value);
	}
	const hanldeCloseAction = () => {
		handleSideActionChange("")
		setFocusAction(null)
	}
	const hanldeChoiceClick = (state: boolean, index: number) => {
		setContentBlocks((prevBlocks) => {
			if (!prevBlocks) return null;

			const updatedBlocks = [...prevBlocks];
			updatedBlocks[index] = { ...updatedBlocks[index], isMinimize: state };

			return updatedBlocks;
		});
	}

	const handleAltTextChange = (event: React.ChangeEvent<HTMLInputElement>, index: number) => {
		setContentBlocks((prevBlocks) => {
			if (!prevBlocks) return null;

			const updatedBlocks = [...prevBlocks];
			console.log(event.target.value)
			updatedBlocks[index] = { ...updatedBlocks[index], AltText: event.target.value };

			return updatedBlocks;
		});
	}
	const hanldeAddBlock = (type: string) => {
		let newBlock: contentBlock = {
			type: type,
			data: '',
			AltText: '',
			isMinimize: false
		}

		setContentBlocks(prevBlocks => prevBlocks ? [...prevBlocks, newBlock] : [newBlock]);
	}

	const hanldeContinuteClick = () => {
		window.location.href = '/profile?s=add'
	}

	return (
		<div id="Shot-edit">
			<div className="body-edit">
				<div className="control-btn">
					<button type="button" className='btn cancel'>Cancel</button>
					<div className="save-continute">
						<button type="button" className='btn save'>Save As Draft</button>
						<button type="button" onClick={hanldeContinuteClick} className='btn continute'>Continute</button>
					</div>
				</div>
				{contentBlocks == null ? (
					<div className="content">
						<p className="title">What have you been working on?</p>
						<input type="file" ref={fileInputRef} onChange={handleAddFile} />
						<div className="media-input" onClick={handleMediaClick}>
							<div className="media-content-show">
								<img src={image} alt="" className='i-show' />
								<p className='title-show'>Drag and drop an image, or <span>Browse</span></p>
								<p className="description">
									Minimum 1600px width recommended. Max 10MB each (20MB for videos)
								</p>
								<ul className="upload-requirements-containers">
									<li className="upload-requirement">High resolution images (png, jpg, gif)</li>
									<li className="upload-requirement">Animated gifs</li>
									<li className="upload-requirement">Videos (mp4)</li>
									<li className="upload-requirement">Only upload media you own the rights to</li>
								</ul>
							</div>
						</div>
					</div>
				) : (
					<div className="page-content">
						<div className="content-title">
							<textarea className="title"
								rows={1} maxLength={64}
								placeholder='Give me a name'
								value={titleShot}
								onChange={(e) => { hanldeTitleChange(e) }} />
						</div>
						<div className="content-blocks-container">
							{contentBlocks?.map((contentBlock, key) => {
								return (
									<>
										{contentBlock.type == "image" && (
											<div className={`content-block ${key == 0 ? 'first' : ''}`} key={key}>
												<div
													className={`block-hover ${key === focusAction ? 'focus-block' : ''}`}
													onClick={() => {
														setFocusAction(key)
														setSideAction("image")
													}}>
													{contentBlock.data !== '' ? (
														<div className="image-block" style={contentBlock.isMinimize ? { width: '752px' } : { width: '1024px' }}>
															<img src={contentBlock.data} alt="" />
														</div>
													) : (
														<div className="blank-image">
															<p>Drag and drop media, or Browse</p>
														</div>
													)}
												</div>
											</div>
										)}
										{contentBlock.type == "text" && (
											<div className="content-block" key={key}>
												<div
													className={`block-hover ${key === focusAction ? 'focus-block' : ''}`}
													onClick={() => {
														setFocusAction(key)
														setSideAction("text")
													}}>
													<div className={`text-block ${textFocus === key ? 'focus' : ''}`}>
														<ReactQuill
															theme="snow"
															value={contentBlock.data}
															onChange={(e) => handleDataChange(e, key)}
															modules={modules}
															onFocus={() => { setTextFocus(key) }}
															onBlur={() => { setTextFocus(-1) }}
														/>
													</div>
												</div>
											</div>
										)}
										{contentBlock.type == "video" && (
											<div className="content-block" key={key}>
												<div
													className={`block-hover ${key === focusAction ? 'focus-block' : ''}`}
													onClick={() => {
														setFocusAction(key)
														setSideAction("video")
													}}>
													{contentBlock.data !== '' ? (
														<div className="video-block" style={contentBlock.isMinimize ? { width: '752px' } : { width: '1024px' }}>
															<video className='video-play' src={contentBlock.data} loop={true} autoPlay={true} />
														</div>
													) : (
														<div className="blank-image">
															<p>Drag and drop media, or Browse</p>
														</div>
													)}
												</div>
											</div>
										)}
									</>
								)
							})}
						</div>
						<div className="insert-container">
							<button className="insert btn" onClick={() => { setSideAction("insert") }}>
								<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><path fill="currentColor" d="M13 6a1 1 0 1 0-2 0v5H6a1 1 0 1 0 0 2h5v5a1 1 0 1 0 2 0v-5h5a1 1 0 1 0 0-2h-5z" /></svg>
								<span>Insert Block</span>
							</button>
						</div>
					</div>
				)}
			</div>
			{contentBlocks != null && (
				<>
					<div className="custom-toolbar" style={sideAction === "text" ? {} : { display: 'none' }}>
						<div className="fixed">
							<div className="custome-edior-tool">
								<p className="close"
									onClick={() => { hanldeCloseAction() }}>Close</p>
								<p className='title'><RxText /> Text Block</p>
								<div className='font-group'>
									<p className="font-title">Font</p>
									<select className="ql-header">
										<option value="1"></option>
										<option value="2"></option>
										<option selected></option>
									</select>
									<div className="button-group-font">
										<button className="ql-bold"></button>
										<button className="ql-italic"></button>
										<button className="ql-underline"></button>
									</div>
								</div>
								<div className="align-group">
									<p className="align-title">Alignment</p>
									<div className="button-group-align">
										<button className="ql-align left ql-active" value=""></button>
										<button className="ql-align center" value="center"></button>
										<button className="ql-align right" value="right"></button>
									</div>
								</div>
							</div>
						</div>
					</div>
					{(sideAction === "image" || sideAction === "video") && (
						<div className="custom-toolbar">
							<div className="fixed">
								<p className="close"
									onClick={() => { hanldeCloseAction() }}>Close</p>
								<p className='title'>
									{sideAction === "image" ? (
										<><RxImage /> Image</>
									) : (
										<><RxVideo /> Video</>
									)}
								</p>
								<div className="media-group">
									<p className="font-title image">Media</p>
									<input type="file"
										className="file-change"
										ref={fileInputSideRef}
										onChange={(e) => handleChangeImage(e)}
										style={{ display: 'none' }} />
									{contentBlocks && focusAction !== null && contentBlocks[focusAction].data !== '' ? (
										<div className="border-image">
											{sideAction === "image" ? (
												<img className='media-change'
													src={contentBlocks && focusAction !== null
														? contentBlocks[focusAction]?.data : ""}
													alt="" />
											) : (
												<video className='media-change'
													src={contentBlocks && focusAction !== null
														? contentBlocks[focusAction]?.data : ""}
													loop={true} autoPlay={true} />
											)}
											<div className="chang-rem">
												<button className="change img-btn" onClick={handleChangeClick}>Change</button>
												<button className="remove img-btn" >Remove</button>
											</div>
										</div>
									) : (
										<div className="input-blank-img-container">
											<button className="select-media" onClick={handleChangeClick}>Select Media</button>
											<p className='append-title'>or drop media to upload</p>
										</div>
									)}
								</div>
								<div className="alt-group">
									<p className="font-title image">Alt Text</p>
									<input type="text" name="" id="" className="input-alt"
										placeholder='Enter alt text...'
										value={contentBlocks && focusAction !== null && contentBlocks[focusAction]?.AltText ? contentBlocks[focusAction]?.AltText : ''}
										onChange={(e) =>
											focusAction != null ? handleAltTextChange(e, focusAction) : {}} />
								</div>
								<div className="layout-group">
									<p className="font-title image">Layout</p>
									<div className="buttons-choice">
										<button
											className={`minimize layout-btn ${contentBlocks && focusAction !== null && contentBlocks[focusAction]?.isMinimize
												? 'active' : ''}`}
											onClick={() => { focusAction != null ? hanldeChoiceClick(true, focusAction) : {} }}>
											<svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 16 16"><path fill="currentColor" fill-rule="evenodd" d="M14.78 2.28a.75.75 0 0 0-1.06-1.06L10.5 4.44V2.75a.75.75 0 0 0-1.5 0V7h4.25a.75.75 0 0 0 0-1.5h-1.69zM5.5 11.56v1.69a.75.75 0 0 0 1.5 0V9H2.75a.75.75 0 0 0 0 1.5h1.69l-3.22 3.22a.75.75 0 1 0 1.06 1.06z" clip-rule="evenodd" /></svg>
										</button>
										<button
											className={`maximize layout-btn ${contentBlocks && focusAction !== null && contentBlocks[focusAction]?.isMinimize
												? '' : 'active'}`}
											onClick={() => { focusAction != null ? hanldeChoiceClick(false, focusAction) : {} }}>
											<svg xmlns="http://www.w3.org/2000/svg" width="14" height="14" viewBox="0 0 16 16"><path fill="currentColor" fill-rule="evenodd" d="M15 5.25a.75.75 0 0 1-1.5 0V3.56l-3.22 3.22a.75.75 0 1 1-1.06-1.06l3.22-3.22h-1.69a.75.75 0 0 1 0-1.5H15zM3.81 13.5l2.97-2.97a.75.75 0 1 0-1.06-1.06L2.5 12.69v-1.94a.75.75 0 0 0-1.5 0V15h4.25a.75.75 0 0 0 0-1.5z" clip-rule="evenodd" /></svg>
										</button>
									</div>
								</div>
							</div>
						</div>
					)}
					{sideAction === "insert" && (
						<div className="custom-toolbar">
							<div className="fixed">
								<p className="close"
									onClick={() => { hanldeCloseAction() }}>Close</p>
								<p className='title'>Insert Block</p>
								<div className="basic-group">
									<p className="font-title">Basic</p>
									<div className="type-block text" onClick={() => { hanldeAddBlock("text") }}>
										<RxText />
										<span>Text</span>
									</div>
									<div className="type-block image" onClick={() => { hanldeAddBlock("image") }}>
										<RxImage />
										<span>Image</span>
									</div>
									<div className="type-block video" onClick={() => { hanldeAddBlock("video") }}>
										<RxVideo />
										<span>Video</span>
									</div>
								</div>
								<div className="rich-media-group">
									<p className="font-title">Rich Media</p>
									<div className="type-block">
										<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24"><g fill="none" stroke="currentColor" stroke-width="1.5"><path d="M6 8c0-2.828 0-4.243.879-5.121C7.757 2 9.172 2 12 2s4.243 0 5.121.879C18 3.757 18 5.172 18 8v8c0 2.828 0 4.243-.879 5.121C16.243 22 14.828 22 12 22s-4.243 0-5.121-.879C6 20.243 6 18.828 6 16z" /><path d="M18 19.5c1.4 0 2.1 0 2.635-.273a2.5 2.5 0 0 0 1.092-1.092C22 17.6 22 16.9 22 15.5v-7c0-1.4 0-2.1-.273-2.635a2.5 2.5 0 0 0-1.092-1.093C20.1 4.5 19.4 4.5 18 4.5m-12 15c-1.4 0-2.1 0-2.635-.273a2.5 2.5 0 0 1-1.093-1.092C2 17.6 2 16.9 2 15.5v-7c0-1.4 0-2.1.272-2.635a2.5 2.5 0 0 1 1.093-1.093C3.9 4.5 4.6 4.5 6 4.5" opacity="0.5" /></g></svg>
										<span>Gallery</span>
									</div>
								</div>
							</div>
						</div>
					)}
				</>
			)}
		</div >
	)
}

export default ShotEdit;
