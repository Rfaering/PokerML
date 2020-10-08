from setuptools import setup, find_packages

setup(
    name='PokerML',
    version='0.1.0',
    description='Sample package for Python-Guide.org',    
    author='Rasmus Færing Larsen',
    author_email='rfaering@gmail.com',        
    packages=find_packages(exclude=('tests', 'docs'))
)

