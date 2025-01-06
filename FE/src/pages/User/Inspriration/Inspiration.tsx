import React from 'react'
import Header from '../../../components/Header/Header'
import Explore from '../../../components/Cards/Explore/Explore'
import D1 from '../../../assets/D1.jpg'
import D2 from '../../../assets/D2.jpg'
import D3 from '../../../assets/D3.jpg'
import D4 from '../../../assets/D4.jpg'
import D5 from '../../../assets/D5.jpg'
import Tag from '../../../components/Cards/Tag/Tag'
import './Inspiration.css'

const Inspiration = () => {

    const exp = [
        {username: 'Otaku', liked: 61, viewed: 126, img: D1, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D2, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D3, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D4, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D5, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D1, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D2, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D3, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D4, topic: ['Web design']},
        {username: 'Otaku', liked: 61, viewed: 126, img: D5, topic: ['Web design']},
    ]

    const tags = [
        {tag: 'Landing Page'},
        {tag: 'Web design'},
        {tag: 'User interface'},
    ]

    return (
        <div id='inspiration'>
            <Header />
            <div className="page-content">
                <form className="search-filter section">
                    <div className="title">
                        <p>Explore work from the most talented Vietnamese designers</p>
                        <p>Ready to bring your vision to life with local expertise and creative excellence</p>
                    </div>
                    <div className="search">
                        <input type="text" placeholder='Looking for inspiration?'/>
                        <button type='submit' name='search'>Search</button>
                    </div>
                    <div className="trending">
                        <p>Trending:</p>
                        {tags.map((tag, index) =>
                            <Tag tag={tag.tag}/>
                        )}
                    </div>
                </form>
                <div className="inspis">
                    <div className="inspis-container">
                        {exp.map((exp, index) =>
                            <Explore username={exp.username} liked={exp.liked} viewed={exp.viewed} img={exp.img} topic={exp.topic} />
                        )}
                    </div>
                </div>
            </div>
        </div>
    )
}

export default Inspiration